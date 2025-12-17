namespace QualIQon 

open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model

    module TIC = 
        let customCulture: Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() :?> Globalization.CultureInfo
        customCulture.NumberFormat.NumberDecimalSeparator <- "."
        System.Threading.Thread.CurrentThread.CurrentCulture <- customCulture

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
                            Text ="<b>Total Ion Chromatogram of <i>Chlamydomonas reinhardtii<i> data<b>", 
                            XAnchor = StyleParam.XAnchorPosition.Center, 
                            Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 28, Color = Color.fromString "Black"))
                    ),
                    Font = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black"))
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


        let filesToMassSpectrum (directoryName: string) =
            let directorypath = (String.concat "" [| "./arc/runs"; directoryName; "/mzml" |])
            let allData  = System.IO.Directory.GetFiles (directorypath, "*.mzML")
            let createResultFolderMis = 
                System.IO.Directory.CreateDirectory ((String.concat "" [|"./arc/runs"; directoryName; "/Results/TIC"|]))
            let exe = 
                allData
                |> Array.map (fun x -> 
                let inReaderMS = new MzMLReader(x)
                let inReaderPeaks = new MzMLReader(x)
                let inRunID  = Core.MzIO.Reader.getDefaultRunID inReaderMS 

                let allSpectra = inReaderMS.ReadMassSpectra inRunID
                let intensityRetentionTime = 
                    allSpectra
                    |> Seq.choose (fun ms ->
                        match MzIO.Processing.MassSpectrum.getMsLevel ms with
                        | 1 -> 
                            Some(
                                    MzIO.Processing.MassSpectrum.getScanTime ms,
                                    PeakArray.mzIntensityArrayOf
                                        ((inReaderPeaks).getSpecificPeak1DArraySequential ms.ID)
                                    |> snd
                                    |> Array.sum
                            )
                        | _ -> None
                    )
                    |> Seq.toArray
                    |> Seq.indexed
                    |> Seq.toArray
                
                let transform = 
                    intensityRetentionTime
                    |> Array.map (fun (x, (y,z)) -> 
                        (x,y,z))
                transform)
                
            let charts = 
                exe
                |> Array.map (fun x -> 
                    let charti = 
                        Chart.Line3D (xyz= x, LineWidth = 5., MarkerColor = Color.fromColorScaleValues [ 0; 1; 2 ])
                        |> Chart.withTemplate layout 
                        |> Chart.withSize (1400,1200)
                        |> Chart.withYAxisStyle(TitleText = "Y",Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")) )
                        |> Chart.withXAxisStyle(TitleText = "X",Id = StyleParam.SubPlotId.Scene 1, TitleStandoff = 5, TitleFont = (Font.init (Family = StyleParam.FontFamily.Arial, Size= 22, Color = Color.fromString "Black")))
                        |> Chart.withZAxisStyle (TitleText = "Z")
                    charti)
            charts 
            |> Chart.combine 
            |> Chart.saveHtml(String.concat "" [| "./arc/runs"; directoryName; "/Results/TIC/TIC-plot" |])
        filesToMassSpectrum 

