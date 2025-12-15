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
                    
                let templateChlamy = Template.init (majorLayout, traceLayout)
                templateChlamy
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

module MS1MapPlot = 
    open MassspecFiles 
        let inputData (dirName: string) = MassspecFiles.filesToMassSpectrum dirName
        let calc (path:string) = 
            let input  = inputData path
            let create bandwidth (data: pieces[]) =            
                let halfBw = bandwidth / 1.0       
                data
                |> Seq.groupBy (fun x -> floor (x.RetentionTime / bandwidth)) 
                |> Seq.map (fun (k,values) ->                                 
                    if k < 0. then
                        ((k  * bandwidth) + halfBw, values)   
                    else
                        ((k + 1.) * bandwidth) - halfBw, values)  
                |> Map.ofSeq

            //bins for inner m/Z values
            let findMzBins (minMax: float * float) = 
                let min,max = minMax
                abs(ceil max - floor min) / 50.

            //collect all m/z -> Min & Max for Binning 
            let minMaxAllSpectra  =
                let outerArray = 
                    input
                    |> Array.map (fun outer -> 
                        let data = 
                            outer
                            |> Array.map (fun x -> 
                                x
                                |> Array.map (fun (x,y)->  
                                    y
                                    |> fst
                                )
                            )
                        let min = 
                            data 
                            |> Array.map(fun x -> 
                                x
                                |> Array.min)
                            |> Array.min
                            
                        let max = 
                            data 
                            |> Array.map(fun x -> 
                                x
                                |> Array.max)
                            |> Array.max
                        min, max )
                outerArray
            let concatMzBins = minMaxAllSpectra |> Array.map (findMzBins)

            let createMzBins bandwidth (data: pieces[]) =            
                let halfBw = bandwidth / 1.0       
                data
                |> Seq.groupBy (fun x -> floor (x.mz / bandwidth)) 
                |> Seq.map (fun (k,values) ->                                 
                    if k < 0. then
                        ((k  * bandwidth) + halfBw, values)   
                    else
                        ((k + 1.) * bandwidth) - halfBw, values)  
                |> Map.ofSeq

            let dataXIC  = 
                let accsessParameters = 
                    input 
                    |> Array.map (fun z -> 
                        z                        |> Array.collect (fun x -> 
                            x
                            |> Array.map (fun (y ,(z,a))-> 
                                {RetentionTime = y;
                                mz = z;
                                intensity = a})))
                //anwendung der binning funktionen
                let binRT = 
                    accsessParameters
                    |> Array.map (fun x ->
                        x
                        |> create 1
                        |> Map.toArray)
                let binMz = 
                    binRT 
                    |> Array.map(fun z -> 
                            z
                            |> Array.map (fun (x,y) -> 
                                let muhaha = 
                                    y
                                    |> Seq.toArray
                                    |> createMzBins concatMzBins  
                                    |> Map.toArray
                                    |> Array.map (fun (x,y)-> 
                                        //Berechnung der SUmme der Intensitäten pro m/Z bin 
                                        let calcSumIntensity = 
                                            y
                                            |> Seq.toArray
                                            |> Array.map (fun x -> x.intensity)
                                            |> Array.sum
                                        x, calcSumIntensity)
                                x, muhaha
                        )
                    )
                binMz

            let Heatmap = 
                let extractIntensity = 
                    dataXIC
                    |> Array.map (fun x -> 
                        x
                        |> fun (x,y)-> 
                            y
                            |> Array.map (fun x -> snd x))  
                let chart = 
                    Chart.Heatmap (zData = extractIntensity)
                chart 
        

