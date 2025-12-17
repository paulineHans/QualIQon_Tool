namespace QualIQon

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects

module Corr_Ratio =     

    let customCulture: Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() :?> Globalization.CultureInfo
    customCulture.NumberFormat.NumberDecimalSeparator <- "."
    System.Threading.Thread.CurrentThread.CurrentCulture <- customCulture

    let layout = 
            let axsisLayout () =
                LinearAxis.init (
                ShowLine = true,
                ZeroLine = false,
                TickLabelStep = 1,
                ShowTickLabels = true,
                AutoRange = StyleParam.AutoRange.True,
                TickFont = (Font.init (Size = 20))  
            )
            let majorLayout =    
                Layout.init (
                Title.init(
                    Text ="<b>sample vs sample correlation: 14N to 15N: <i>Chlamydomonas reinhardtii<i> quant Files<b>", 
                    XAnchor = StyleParam.XAnchorPosition.Center, 
                    Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 26, Color = Color.fromString "Black")))
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
    let ratio (data : (string * ((string * int * bool) * float) array) array)= 
        let files = data |> Array.map (fun x -> (fst x).Split("TM").[1])
        let allData = data |> Array.map snd 
        let calc = 
            allData
            |> Array.map (fun x -> 
                x
                |> Array.filter (fun x -> not (isNan(snd x)))
                |> Array.map (fun z -> fst z, log2 (snd z + 1.)))
        let calcPearson = 
            calc 
            |> Array.map (fun x -> 
                calc 
                |> Array.map (fun y -> 
                    let intersect = 
                        Set.intersect
                            (x |> Array.map fst |> Set.ofArray)
                            (y |> Array.map fst |> Set.ofArray)
                    let xFilter = 
                        x |> Array.filter (fun (x,y ) -> 
                            intersect |> Set.contains x) |> Array.map snd
                    let yFilter= 
                        y |> Array.filter (fun (x,y)-> intersect |> Set.contains x) |> Array.map snd
                    Correlation.Seq.pearson yFilter xFilter))



        let heatMapR = 
            Chart.Heatmap(zData = calcPearson, colNames = files, rowNames = files, ColorScale = StyleParam.Colorscale.Portland)

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
                    Text ="<b>sample vs sample correlation: 14N to 15N: <i>Chlamydomonas reinhardtii<i> MS runs<b>", 
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
        let stylingRatio = 
            heatMapR 
            |> Chart.withSize(2200,2000)
            |> Chart.withTemplate layout
            |> Chart.withMarginSize (400,250,130,180)
            |> Chart.withXAxisStyle (TitleText ="<b>MS run<b>", TitleStandoff = 20)
            |> Chart.withYAxisStyle (TitleText="<b>MS run<b>", TitleStandoff = 10)
            
        stylingRatio
    ratio

    //adapterfunction for MaxQuant, FragPipe & ProteomIQon
    let proteomIQonRatio (path: string) = 
        let allData = 
            let fileNamesL = System.IO.Path.GetFileNameWithoutExtension path
            let files = 
                Deedle.Frame.ReadCsv(path,true,true,separators="\t")
            let data = 
                files
                |> Frame.indexRowsUsing (fun x -> 
                    x.GetAs<string>("StringSequence"),
                    x.GetAs<int>("Charge"),
                    x.GetAs<bool>("GlobalMod")
                )
            let extractLight: Series<string*int*bool,float> = 
                data
                |> Frame.getCol "Quant_Light"
            let extractHeavy: Series<string*int*bool,float> = 
                data 
                |> Frame.getCol "Quant_Heavy"
            let divide = 
                extractLight/extractHeavy
                |> Series.observations
                |> Seq.toArray
                |> Array.map (fun (x,y)-> x, y |> float)
            fileNamesL, divide 
        allData

    let matchingForFile_Ratio (arguments: string) =  
        match arguments with 
        | "ProteomIQon" -> proteomIQonRatio 
        | _ -> failwith "unknown Pattern"

    let rec searchFilesRatio (directoryName: string) (fileName: string) : string[] =
        // Get files in the current directory that match the filename.
        let currentFiles = Directory.GetFiles(directoryName,fileName )
        
        // Get all subdirectories.
        let subDirectories = Directory.GetDirectories(directoryName)
        
        // Recursively search each subdirectory.
        let subDirFiles =
            subDirectories
            |> Array.collect (fun subDir -> searchFilesRatio subDir fileName)
        
        // Combine files from the current directory and all subdirectories.
        Array.append currentFiles subDirFiles


    let matchingDirecoryPath_Ratio (sndArg : string) = 
        match sndArg with
        | "ProteomIQon" -> "*.quant"
        | _ -> "unknown Format"

    //read-in files
    let finalHeatmaRatio (directoryName: string) pipeline =
        let directorypath_Ratio = (String.concat "" [| "./arc/runs" ;directoryName|])
        let searchpattern = matchingDirecoryPath_Ratio pipeline
        let allData_Ratio = searchFilesRatio directorypath_Ratio searchpattern
        let paramsArray = 
            allData_Ratio
            |> Array.map (fun x ->  matchingForFile_Ratio pipeline x) 
            |> Array.sort         
        let execution = paramsArray |> ratio
        let createResultsFolderLight  = 
            System.IO.Directory.CreateDirectory (String.concat ""[|"./arc/runs";directoryName; "/Results/Corr_Ratio"|])
        
        // saving
        let savingR = 
            execution
            |> fun x -> 
                x 
                |> Chart.saveHtml (String.concat "" [| "./arc/runs" ;directoryName;"/Results/Corr_Ratio/Heatmap_Correlation_Ratio" |])
        savingR
    