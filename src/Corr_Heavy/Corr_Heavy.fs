namespace QualIQon

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects

module CorrHeavy =     
    
    let customCulture: Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() :?> Globalization.CultureInfo
    customCulture.NumberFormat.NumberDecimalSeparator <- "."
    System.Threading.Thread.CurrentThread.CurrentCulture <- customCulture

    let quantHeavy (data : (string * ((string * int * bool) * float) array) array) = 
        let file = data |> Array.map (fun x -> (fst x).Split("TM").[1])
        let allData = data |> Array.map (fun x -> snd x )
        let getQH = 
            allData
            |> Array.map (fun x -> 
                x
                |> Array.filter (fun x -> not (isNan(snd x)))
                |> Array.map (fun z -> fst z, log2 (snd z + 1.) 
                )
            )
        let getSeqForPearsonQH  = 
            getQH
            |> Array.map (fun x -> 
                getQH
                |> Array.map (fun y -> 
                    let intersect = 
                        Set.intersect
                            (x |> Array.map fst |> Set.ofArray)
                            (y |> Array.map fst |> Set.ofArray)
                    let xfilter = 
                        x |> Array.filter (fun (x,y) -> 
                        intersect |> Set.contains x) |> Array.map snd 
                    let yFilter = 
                        y|> Array.filter (fun (x,y) -> intersect |> Set.contains x) |> Array.map snd 
                    Correlation.Seq.pearson yFilter xfilter
                ) 
            )
        //Heatmap quant_heavy files
        let heatMap_Heavy= 
            Chart.Heatmap(zData = getSeqForPearsonQH, colNames = file, rowNames = file, ColorScale = StyleParam.Colorscale.Portland)
        let layout = 
            let axsisLayout () =
                LinearAxis.init (
                ShowLine = true,
                ZeroLine = false,
                TickLabelStep = 1,
                ShowTickLabels = true,
                AutoRange = StyleParam.AutoRange.True,
                TickFont = (Font.init (Size = 20, Family = StyleParam.FontFamily.Arial)) 
            )
            let majorLayout =    
                Layout.init (
                Title.init(
                    Text ="<b>sample vs sample correlation: 15N to 15N: <i>Chlamydomonas reinhardtii<i> MS runs<b>", 
                    XAnchor = StyleParam.XAnchorPosition.Center, 
                    Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black"))
                    ), 
                    Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 25, Color = Color.fromString "Black"))
                )
                |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))
            let traceLayout = 
                    [Trace2D.initScatter(
                            Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true))
                        )
                    ]
                
            let templateChlamy = Template.init (majorLayout, traceLayout)
            templateChlamy
        //styling
        let stylingQuantHeavy = 
            heatMap_Heavy 
            |> Chart.withSize(2200,2000)
            |> Chart.withTemplate layout
            |> Chart.withMarginSize (400,250,130,180)
            |> Chart.withXAxisStyle (TitleText ="<b>MS run<b>", TitleStandoff = 20)
            |> Chart.withYAxisStyle (TitleText="<b>MS run<b>", TitleStandoff = 10)   
        stylingQuantHeavy

    //adapterfunction for MaxQuant, FragPipe & ProteomIQon
    let proteomIQonHeavy (path: string) = 
        let fileNames = System.IO.Path.GetFileNameWithoutExtension path
        let files = 
            Deedle.Frame.ReadCsv(path,true,true,separators="\t")
        let data = 
            files 
            |> Frame.indexRowsUsing (fun x -> 
                x.GetAs<string>("StringSequence"),
                x.GetAs<int>("Charge"),
                x.GetAs<bool>("GlobalMod")
            )
            |> Frame.getCol "Quant_Heavy"
            |> Series.observations
            |> Seq.toArray
            |> Array.map (fun (x,y) -> x, y |> float)
        fileNames, data

    let matchingForFile_Heavy (arguments: string) =  
        match arguments with 
        | "ProteomIQon" -> proteomIQonHeavy 
        | _ -> failwith "unknown Pattern"

    let rec searchFilesQH (directoryName: string) (fileName: string) : string[] =
        // Get files in the current directory that match the filename.
        let currentFiles = Directory.GetFiles(directoryName,fileName )
        
        // Get all subdirectories.
        let subDirectories = Directory.GetDirectories(directoryName)
        
        // Recursively search each subdirectory.
        let subDirFiles =
            subDirectories
            |> Array.collect (fun subDir -> searchFilesQH subDir fileName)
        
        // Combine files from the current directory and all subdirectories.
        Array.append currentFiles subDirFiles

    let matchingDirecoryPath_heavy (sndArg : string) = 
        match sndArg with
        | "ProteomIQon" -> "*.quant"
        | _ -> "unknown Format"



    //read-in all files 
    let finalHeatmapQuantHeavy (directoryName: string) pipeline =
        let directorypath_QH = (String.concat "" [| "./arc/runs" ;directoryName|])
        let searchpattern = matchingDirecoryPath_heavy pipeline
        let allData_QH = searchFilesQH directorypath_QH searchpattern
        let paramsArray = 
            allData_QH
            |> Array.map (fun x ->  matchingForFile_Heavy pipeline x) 
            |> Array.sort         
        let executionQH = paramsArray |> quantHeavy
        let createResultsFolderLight  = 
            System.IO.Directory.CreateDirectory (String.concat ""[|"./arc/runs";directoryName; "/Results/Corr_Heavy"|])
        
        // saving
        let savingQH = 
            executionQH
            |> fun x -> 
                x 
                |> Chart.saveHtml (String.concat "" [| "./arc/runs" ;directoryName;"/Results/Corr_Heavy/Heatmap_Correlation_QuantHeavy" |])
                
        savingQH