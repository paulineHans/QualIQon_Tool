namespace QualIQon

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects

module Corr_Light =     

        
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
                    Text ="<b>sample vs sample correlation: 14N to 14N: <i>Chlamydomonas reinhardtii<i> quant Files<b>", 
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



    // get QuantLight 
    let quantLight (data : (string * ((string * int * bool) * float) array) array) = 
        let file = data |> Array.map (fun x -> (fst x).Split("TM").[1])
        let allData = data |> Array.map (fun x -> snd x )
        let getQuantLight = 
            allData
            |> Array.map (fun x -> 
                x
                |> Array.filter (fun x -> not (isNan(snd x)))
                |> Array.map (fun z -> fst z, log2 (snd z + 1.) 
                )
            )
        
        // calculation pearson correlation between all files 
        let getSeqForPearson  = 
            getQuantLight
            |> Array.map (fun x -> 
                getQuantLight
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
        //creating Heatmap for quant light files 
        let heatMapQL = 
            Chart.Heatmap(zData = getSeqForPearson, colNames = file, rowNames = file, ColorScale = StyleParam.Colorscale.Portland)
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
                    Text ="<b>sample vs sample correlation: 14N to 14N: <i>Chlamydomonas reinhardtii<i> MS runs<b>", 
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
        let stylingQuantLight = 
            heatMapQL 
            |> Chart.withSize(2200,2000)
            |> Chart.withTemplate layout
            |> Chart.withMarginSize (400,250,130,180)
            |> Chart.withXAxisStyle (TitleText ="<b>MS run<b>", TitleStandoff = 20)
            |> Chart.withYAxisStyle (TitleText="<b>MS run<b>", TitleStandoff = 10)
        stylingQuantLight
    quantLight

    // adapterfunction for MaxQuant, FragPipe & ProteomIQon
    let proteomIQon (path: string) = 
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
            |> Frame.getCol "Quant_Light"
            |> Series.observations
            |> Seq.toArray
            |> Array.map (fun (y, x) -> y, x |> float)
        fileNames, data
                                                                                                                                                                                                                                                                        
    let maxQuantToParams (path :string)= 
        let files = Deedle.Frame.ReadCsv (path, hasHeaders =  true,  separators = "\t")
        let columns = 
            files 
            |> Frame.indexRowsUsing (fun x -> 
                x.GetAs<string>("Sequence"),
                x.GetAs<int>("Charges"),
                x.GetAs<bool>("Labeling State")
            )
            |> Frame.getCol "Intensity"
            |> Series.observations
            |> Seq.toArray 
            |> Array.map (fun (x, y) -> x, y |> float)
        "Quant_Light",columns
    

    let FragPipeToParams(path: string) = 
        let files = Deedle.Frame.ReadCsv (path, hasHeaders =  true,  separators = "\t")
        let columns = 
            files 
            |> Frame.indexRowsUsing (fun x -> 
                x.GetAs<string>("Peptide Sequence"),
                x.GetAs<int>("Charge"),
                x.GetAs<bool>("Label Count"))
            |> Frame.getCol "Intensity"
            |> Series.observations
            |> Seq.toArray
            |> Array.map (fun (x,y) -> x,y |> float)
        "Quant_Light",columns

    let matchingForFile (arguments: string) =  
        match arguments with 
        | "ProteomIQon" -> proteomIQon 
        | "MaxQuant" -> maxQuantToParams
        | "FragPipe" -> FragPipeToParams
        | _ -> failwith "unknown Pattern"


    let rec searchFiles (directoryName: string) (fileName: string) : string[] =
        // Get files in the current directory that match the filename.
        let currentFiles = Directory.GetFiles(directoryName,fileName )
        
        // Get all subdirectories.
        let subDirectories = Directory.GetDirectories(directoryName)
        
        // Recursively search each subdirectory.
        let subDirFiles =
            subDirectories
            |> Array.collect (fun subDir -> searchFiles subDir fileName)
        
        // Combine files from the current directory and all subdirectories.
        Array.append currentFiles subDirFiles
    
    let matchingDirecoryPath (sndArg : string) = 
        match sndArg with
        | "ProteomIQon" -> "*.quant"
        | "MaxQuant" -> "evidence.txt"
        | "FragPipe" -> "ion_label_quant.tsv"
        | _ -> "unknown Format"
    //read-in the files 
    let finalHeatmapQuantLight (directoryName: string) pipeline =
        let directorypath_QuantLight = (String.concat "" [| "./arc/runs" ;directoryName|])
        let searchpattern = matchingDirecoryPath  pipeline
        let allData_QuantLight = searchFiles directorypath_QuantLight searchpattern
        let paramsArray = 
            allData_QuantLight
                |> Array.map (fun x ->  matchingForFile pipeline x)     |> Array.sort
                
        let execution = paramsArray |>  quantLight
        let createResultsFolderLight  = 
            System.IO.Directory.CreateDirectory (String.concat ""[|"./arc/runs";directoryName; "/Results/Corr_Light"|])
        
        // saving
        let saving = 
            execution
            |> fun x -> 
                x 
                    |> Chart.saveHtml(String.concat "" [| "./arc/runs" ;directoryName;"/Results/Corr_Light/Heatmap_Correlation_QuantLight" |])
                
        saving
  