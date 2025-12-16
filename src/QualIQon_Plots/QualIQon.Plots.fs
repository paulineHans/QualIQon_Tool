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

module MisscleavagesPlot = 
    open Misscleavages
        let MC (dirName: string) (pipeline: string) = 
            let inputData = Misscleavages.mcExe dirName pipeline
            let exe = 
                inputData
                |> Array.fold (fun (c : float, acc : list<GenericChart*string>) (x:(string*(ParametersMisscleavages array))) ->  
                        let file = (fst x ).Split("TM").[1]
                        let data = snd x            
                        let groupMC =             
                            data
                            |> Array.map (fun x -> x.MC)
                            |> Array.countBy id 
                            |> Array.sortBy fst
                        let sumAllMC = 
                            groupMC |> Array.sumBy snd |> float
                        let getPerMC = 
                            groupMC |> Array.map (fun (key,count) -> 
                            string key, (((count |> float) /sumAllMC)))
                            |> Array.tail 
                        let maxN = 
                            getPerMC
                            |> Array.maxBy snd 
                            |> snd 
                        let newMax = 
                            if maxN > c then maxN
                            else c
                        let chart = 
                            Chart.Column (
                                keysValues = getPerMC,
                                MarkerColor = Color.fromColors [Color.fromHex("#FFA252"); Color.fromHex("#E31B4C")]
                                ) 
                            |> Chart.withTraceInfo (ShowLegend = false)
                        (newMax, (chart,file) :: acc) 
                    )

                        (0,[])
            let layout = 
                let axsisLayout () =
                    LinearAxis.init (
                        ShowLine = true,
                        ZeroLine = false,
                        TickLabelStep = 1,
                        ShowTickLabels = true,
                        Mirror = StyleParam.Mirror.All,
                        TickFont = (Font.init (Size = 20))    
                    )
                
                let majorLayout =    
                        Layout.init (
                            Title.init(
                                    Text="<b>Overview of the realtive distribution of Misscleavages in <i>Chlamydomonas reinhardtii<i><b>", 
                                    Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black")), 
                                    XAnchor = StyleParam.XAnchorPosition.Center,
                                    AutoMargin = false,
                                    Standoff = 2
                                ),
                            Font = Font.init (Family = StyleParam.FontFamily.Arial, Size = 20, Color = Color.fromString "Black")
                            )
                        |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                        |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))

                let traceLayout = 
                        [Trace2D.initScatter(
                            Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true)))]
                    
                let template = Template.init (majorLayout, traceLayout)
                template
            //miscleavagesHistogram
            let maxY,chartListRaw = exe 
            let chartList,titlList =List.unzip chartListRaw
            let chartListMaxY = 
                chartList
                |> List.map (fun x  -> 
                    x
                    |> Chart.withTemplate layout
                    |> Chart.withYAxisStyle(TitleText = "<b>relative distribution of Misscleavages <b>", MinMax = (0,(maxY *1.1)), TitleStandoff = 5 )
                    |> Chart.withXAxisStyle(TitleText = "<b>Amount of Misscleavages<b>", MinMax = (0,3), TitleStandoff = 5)

                    )  
                    
            let gridCombination = 
                let nRows = 
                    System.Math.Ceiling((chartListMaxY.Length |> float) / 4.)
                chartListMaxY
                |> Chart.Grid ((nRows |> int),6, Pattern = StyleParam.LayoutGridPattern.Coupled, XGap = 0.2, YGap = 0.3)

            let stylingChart = 
                gridCombination
                |> Chart.withSize (2200,2200)
                |> Chart.withMarginSize(300,250,130,180)
            stylingChart

module TICPlot = 
    open TIC 
        let layout = 
            let axsisLayout () =
                LinearAxis.init (
                    ShowLine = true,
                    ZeroLine = false,
                    TickLabelStep = 1,
                    ShowTickLabels = true,
                    Mirror = StyleParam.Mirror.All,
                    TickFont = (Font.init (Size = 20))    
                )
            
            let majorLayout =    
                    Layout.init (
                        Title.init(
                                Text="<b>Total Ion Chromatogram of <i>Chlamydomonas reinhardtii<i> data<b>", 
                                Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black")), 
                                XAnchor = StyleParam.XAnchorPosition.Center,
                                AutoMargin = false,
                                Standoff = 2
                            ),
                        Font = Font.init (Family = StyleParam.FontFamily.Arial, Size = 20, Color = Color.fromString "Black")
                        )
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))

            let traceLayout = 
                    [Trace2D.initScatter(
                        Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true)))]
                
            let template = Template.init (majorLayout, traceLayout)
            template
        let createTICChart (inputData: (float * float * float)[]) =
            Chart.Line3D (xyz = inputData, LineWidth = 5., MarkerColor = Color.fromColorScaleValues [ 0; 1; 2 ])
            |> Chart.withTemplate layout 
            |> Chart.withSize (1400, 1200)
            |> Chart.withYAxisStyle(TitleText = "Intensity", Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")) )
            |> Chart.withXAxisStyle(TitleText = "Retention Time", Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")))
            |> Chart.withZAxisStyle (TitleText = "Index") 


module XICPlot =
    open XIC
    let createXIC  = 
        let collectMinMaxMz (xicDataArray: (float * (float[] * float[]))[][]) =
            let allMz = 
                xicDataArray
                |> Array.collect (fun fileData ->
                    fileData
                    |> Array.collect (fun (rt, (mz, _)) -> mz)
                )
            let minMz = Array.min allMz
            let maxMz = Array.max allMz
            minMz, maxMz

        let calculateMzBinWidth (minMax: float * float) =
            let min, max = minMax
            abs(ceil max - floor min) / 50.

        // Function to bin data by retention time
        let binByRetentionTime (bandwidth: float) (data: pieces[]) =
            let halfBw = bandwidth / 2.0
            data
            |> Seq.groupBy (fun x -> floor (x.RetentionTime / bandwidth))
            |> Seq.map (fun (k, values) ->
                let rtBin =
                    if k < 0. then
                        (k * bandwidth) + halfBw
                    else
                        ((k + 1.) * bandwidth) - halfBw
                rtBin, (values |> Seq.toArray)
            )
            |> Map.ofSeq

        // Function to bin data by m/z within each RT bin

        let binByMz (bandwidth: float) (data: pieces[]) =
            let halfBw = bandwidth / 2.0
            data
            |> Seq.groupBy (fun x -> floor (x.mz / bandwidth))
            |> Seq.map (fun (k, values) ->
                let mzBin =
                    if k < 0. then
                        (k * bandwidth) + halfBw
                    else
                        ((k + 1.) * bandwidth) - halfBw
                let intensitySum = values |> Seq.sumBy (fun x -> x.intensity)
                mzBin, intensitySum
            )
            |> Map.ofSeq
        // Function to create XIC data from raw data

        let createXICData (xicDataArray: (float * (float[] * float[]))[][])   =
            let minMaxMz = collectMinMaxMz xicDataArray
            let mzBinWidth = calculateMzBinWidth minMaxMz
            let allPieces =
                xicDataArray
                |> Array.collect (fun fileData ->
                    fileData
                    |> Array.collect (fun (rt, (mzArray, intArray)) ->
                        Array.zip mzArray intArray
                        |> Array.map (fun (mz, intensity) ->
                            { RetentionTime = rt; mz = mz; intensity = intensity }
                        )
                    )
                )
            let rtBins = binByRetentionTime 1.0 allPieces |> Map.toArray
            rtBins
            |> Array.collect (fun (rt, pieces ) ->
                let mzBins = binByMz mzBinWidth pieces |> Map.toArray
                mzBins |> Array.map (fun (mz, intensity) -> rt, mz, intensity)
            )
        let layout = 
            let axsisLayout () =
                LinearAxis.init (
                    ShowLine = true,
                    ZeroLine = false,
                    TickLabelStep = 1,
                    ShowTickLabels = true,
                    Mirror = StyleParam.Mirror.All,
                    TickFont = (Font.init (Size = 20))    
                )
            
            let majorLayout =    
                    Layout.init (
                        Title.init(
                                Text="<b>Overview of the realtive distribution of Misscleavages in <i>Chlamydomonas reinhardtii<i><b>", 
                                Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black")), 
                                XAnchor = StyleParam.XAnchorPosition.Center,
                                AutoMargin = false,
                                Standoff = 2
                            ),
                        Font = Font.init (Family = StyleParam.FontFamily.Arial, Size = 20, Color = Color.fromString "Black")
                        )
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))

            let traceLayout = 
                    [Trace2D.initScatter(
                        Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true)))]
                
            let template = Template.init (majorLayout, traceLayout)
            template
        // Function to create a 3D line chart from XIC data

        let createXICChart (a: (float * (float array * float array)) array array)  =
            let exe = createXICData a
            Chart.Line3D (xyz = exe, LineWidth = 5., MarkerColor = Color.fromColorScaleValues [0; 1; 2])
            |> Chart.withTemplate layout
            |> Chart.withSize (1400, 1200)
            |> Chart.withYAxisStyle(TitleText = "Intensity", Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")))
            |> Chart.withXAxisStyle(TitleText = "Retention Time", Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")))
            |> Chart.withZAxisStyle (TitleText = "m/z")
        createXICChart

module MS1MapPlot = 
    open MS1Map

    let createMS1Map = 
        let collectMinMaxMz (xicDataArray: (float * (float[] * float[]))[][]) =
            let allMz = 
                xicDataArray
                |> Array.collect (fun fileData ->
                    fileData
                    |> Array.collect (fun (rt, (mz, _)) -> mz)
                )
            let minMz = Array.min allMz
            let maxMz = Array.max allMz
            minMz, maxMz

        let calculateMzBinWidth (minMax: float * float) =
            let min, max = minMax
            abs(ceil max - floor min) / 50.

        // Function to bin data by retention time
        let binByRetentionTime (bandwidth: float) (data: pieces[]) =
            let halfBw = bandwidth / 2.0
            data
            |> Seq.groupBy (fun x -> floor (x.RetentionTime / bandwidth))
            |> Seq.map (fun (k, values) ->
                let rtBin =
                    if k < 0. then
                        (k * bandwidth) + halfBw
                    else
                        ((k + 1.) * bandwidth) - halfBw
                rtBin, (values |> Seq.toArray)
            )
            |> Map.ofSeq

        // Function to bin data by m/z within each RT bin

        let binByMz (bandwidth: float) (data: pieces[]) =
            let halfBw = bandwidth / 2.0
            data
            |> Seq.groupBy (fun x -> floor (x.mz / bandwidth))
            |> Seq.map (fun (k, values) ->
                let mzBin =
                    if k < 0. then
                        (k * bandwidth) + halfBw
                    else
                        ((k + 1.) * bandwidth) - halfBw
                let intensitySum = values |> Seq.sumBy (fun x -> x.intensity)
                mzBin, intensitySum
            )
            |> Map.ofSeq
        // Function to create XIC data from raw data

        let createXICData (xicDataArray: (float * (float[] * float[]))[][]) =
            let minMaxMz = collectMinMaxMz xicDataArray
            let mzBinWidth = calculateMzBinWidth minMaxMz
            let allPieces =
                xicDataArray
                |> Array.collect (fun fileData ->
                    fileData
                    |> Array.collect (fun (rt, (mzArray, intArray)) ->
                        Array.zip mzArray intArray
                        |> Array.map (fun (mz, intensity) ->
                            { RetentionTime = rt; mz = mz; intensity = intensity }
                        )
                    )
                )
            let rtBins = binByRetentionTime 1.0 allPieces |> Map.toArray
            rtBins
            |> Array.collect (fun (rt, pieces ) ->
                let mzBins = binByMz mzBinWidth pieces |> Map.toArray
                mzBins |> Array.map (fun (mz, intensity) -> rt, (mz, intensity))
            )
            
        let layout = 
            let axsisLayout () =
                LinearAxis.init (
                    ShowLine = true,
                    ZeroLine = false,
                    TickLabelStep = 1,
                    ShowTickLabels = true,
                    Mirror = StyleParam.Mirror.All,
                    TickFont = (Font.init (Size = 20))    
                )
            
            let majorLayout =    
                    Layout.init (
                        Title.init(
                                Text="<b>Overview of the realtive distribution of Misscleavages in <i>Chlamydomonas reinhardtii<i><b>", 
                                Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 30, Color = Color.fromString "Black")), 
                                XAnchor = StyleParam.XAnchorPosition.Center,
                                AutoMargin = false,
                                Standoff = 2
                            ),
                        Font = Font.init (Family = StyleParam.FontFamily.Arial, Size = 20, Color = Color.fromString "Black")
                        )
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.XAxis 1), (axsisLayout ()))
                    |> Layout.setLinearAxis ((StyleParam.SubPlotId.YAxis 1), (axsisLayout ()))

            let traceLayout = 
                    [Trace2D.initScatter(
                        Trace2DStyle.Scatter(Marker = Marker.init (AutoColorScale = true)))]
                
            let template = Template.init (majorLayout, traceLayout)
            template
        // Function to create a 3D line chart from XIC data

        let createXICChart (inputData: (float * (float[] * float[]))[][])  =
            let exe = createXICData inputData
            let extractIntensity = 
                exe 
                |> Array.map (fun (x,y) -> y |> snd)
            let buildHeatmap = 
                Chart.Heatmap (zData = [|extractIntensity|])
                |> Chart.withTemplate layout
                |> Chart.withSize (1400, 1200)
                |> Chart.withYAxisStyle(TitleText = "Retention Time", Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")))
                |> Chart.withXAxisStyle(TitleText = "m/z", Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")))
            buildHeatmap
        createXICChart