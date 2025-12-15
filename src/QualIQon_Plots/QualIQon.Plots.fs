namespace QualIQon.Plots

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open QualIQon.IO


module CorrHeavyPlot = 
    open CorrHeavy
        let quantHeavy (dirName: string) (pipeline: string) = 
            let inputData = CorrHeavy.finalHeatmapQuantHeavy dirName pipeline
            let file = inputData |> Array.map (fun x -> (fst x).Split("TM").[1])
            let allData = inputData |> Array.map (fun x -> snd x )
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
                    
                let template = Template.init (majorLayout, traceLayout)
                template
            //styling
            let stylingQuantHeavy = 
                heatMap_Heavy 
                |> Chart.withSize(2200,2000)
                |> Chart.withTemplate layout
                |> Chart.withMarginSize (400,250,130,180)
                |> Chart.withXAxisStyle (TitleText ="<b>MS run<b>", TitleStandoff = 20)
                |> Chart.withYAxisStyle (TitleText="<b>MS run<b>", TitleStandoff = 10)   
            stylingQuantHeavy

module CorrLightPlot = 
    open CorrLight

    // get QuantLight 
    let quantLight (dirName: string) (pipeline: string) = 
        let inputData = CorrLight.finalHeatmapQuantLight dirName pipeline
        let file = inputData |> Array.map (fun x -> (fst x).Split("TM").[1])
        let allData = inputData |> Array.map (fun x -> snd x )
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
                
            let template = Template.init (majorLayout, traceLayout)
            template

        //styling
        let stylingQuantLight = 
            heatMapQL 
            |> Chart.withSize(2200,2000)
            |> Chart.withTemplate layout
            |> Chart.withMarginSize (400,250,130,180)
            |> Chart.withXAxisStyle (TitleText ="<b>MS run<b>", TitleStandoff = 20)
            |> Chart.withYAxisStyle (TitleText="<b>MS run<b>", TitleStandoff = 10)
        stylingQuantLight

module CorrRatioPlot = 
    open CorrRatio
        
        let quantRatio (dirName: string) (pipeline: string) = 
            let inputData = CorrRatio.finalHeatmapQuantRatio dirName pipeline
            let files = inputData |> Array.map (fun x -> (fst x).Split("TM").[1])
            let allData = inputData |> Array.map snd 
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
                    
                let template = Template.init (majorLayout, traceLayout)
                template
            //styling
            let stylingRatio = 
                heatMapR 
                |> Chart.withSize(2200,2000)
                |> Chart.withTemplate layout
                |> Chart.withMarginSize (400,250,130,180)
                |> Chart.withXAxisStyle (TitleText ="<b>MS run<b>", TitleStandoff = 20)
                |> Chart.withYAxisStyle (TitleText="<b>MS run<b>", TitleStandoff = 10) 
            stylingRatio

module CorrelationLightHeavyPlot = 
    open CorrelationLightHeavy
        let CorrLHP (dirName: string) (pipeline: string) = 
                let inputData = CorrelationLightHeavy.finalHeatmapQuantHeavy dirName pipeline
                //files down below 
                let dataCorr = inputData |> Array.map (fun x -> snd x)
                let correlation = dataCorr 
                //bandwidth calculation
                let bandWidthCalculation = 
                    let createBandWidth = 
                        correlation
                        |> Array.map (fun x -> 
                            x
                            |> Array.map (fun y -> 
                                y.R 
                            )
                            |> FSharp.Stats.Distributions.Frequency.create 0.05
                            |> Map.toArray
                        )
                    //% calculation
                    let percentages = 
                        let sum = 
                            createBandWidth
                            |> Array.map (fun x -> 
                                let calculateSum =                    
                                    x
                                    |> Array.sumBy snd
                                    |> float
                                let getSingleCount = 
                                    x
                                    |> Array.map (fun (x,y) -> (x, (y |> float) / calculateSum))
                                getSingleCount
                            )       
                        sum 
                    //file Names 
                    let getFiles = inputData |> Array.map (fun x -> (fst x ).Split("TM").[1]) 
                    
                    //creating Chart
                    let createChart = 
                        percentages
                        |> Array.map2 (fun y x  -> 
                            let building = 
                                Chart.Column (
                                keysValues = x,
                                MarkerColor = Color.fromHex("#FFA252")
                                )
                                |> Chart.withTraceInfo (ShowLegend = false)
                                |> Chart.withYAxisStyle(MinMax= (0,0.57), TitleText = "<b>Relstive abundance<b>", TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")), TitleStandoff = 5, Side = StyleParam.Side.Left)
                                |> Chart.withXAxisStyle (TitleText = "<b>Correlation-Light-Heavy<b>", TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")), TitleStandoff = 5, Side = StyleParam.Side.Bottom)  
                            building, y
                        ) getFiles 
                    createChart
                let stylingChart = 
                    let layout = 
                        let axsisLayout () =
                            LinearAxis.init (
                                // Ticks = StyleParam.TickOptions.Inside,
                                ShowLine = true,
                                ZeroLine = false,
                                TickLabelStep = 1,
                                ShowTickLabels = true,
                                Mirror = StyleParam.Mirror.All,
                                TickFont = (Font.init (Size = 18))
                            )
                        let majorLayout =    
                            Layout.init (
                                Title = 
                                    Title.init(
                                        Text ="<b> Correlation-Light-Heavy plot visualising MS-runs of <i>Chlamydomonas reinhardtii<i> data<b>", 
                                        XAnchor = StyleParam.XAnchorPosition.Center, 
                                        Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black"))
                                ),
                                Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 26, Color = Color.fromString "Black"))
                            )
                            |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                            |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))
                        let traceLayout = 
                                [Trace2D.initScatter(
                                        Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true))
                                    )
                                ]
                                
                        let template = Template.init (majorLayout, traceLayout)
                        template
                        // calculation of the needed amount of Cols and Rows
                    let gridCalculation = 
                        System.Math.Ceiling ((bandWidthCalculation.Length |> float) / 4.)
                        
                    let fining = 
                        bandWidthCalculation
                            |> Array.unzip 
                            |> fun (x,y ) -> 
                                x
                                |> Chart.Grid (nRows =  (gridCalculation |> int), nCols = 6, Pattern = StyleParam.LayoutGridPattern.Coupled)
                                |> Chart.withMarginSize (250, 200, 150, 100)
                                |> Chart.withTemplate layout 
                                |> Chart.withSize (2200,1700)
                    fining
                stylingChart