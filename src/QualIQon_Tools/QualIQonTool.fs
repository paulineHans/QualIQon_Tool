namespace QualIQon.Tool

open System 
open System.IO
open QualIQon.IO 
open QualIQon.Plots 

module createCorrHeavyPlot =
    let HeatmapCorrelationHeavy = QualIQon.Plots.CorrHeavyPlot.quantHeavy

module createCorrLightPlot = 
    let HeatmapCorrelationLight = QualIQon.Plots.CorrLightPlot.quantLight

module createCorrRatioPlot =     
    let HeatmapCorrelationRatio = QualIQon.Plots.CorrRatioPlot.quantRatio

module createCorrelationLightHeavyPlot = 
    let CorrelationLightHeavy = QualIQon.Plots.CorrelationLightHeavyPlot.CorrLHP

module createMisscleavagePlot = 
    let MCPlot = QualIQon.Plots.MisscleavagesPlot.MC

module createProteinIdentificationPlot = 
    let ProteinIdentification = QualIQon.Plots.ProteinIdentificationPlot.PI

module createScoreRefinementPlot = 
    let ScoreRefinement = QualIQon.Plots.ScoreRefinementPlot.SR

module createTICPlot = 
    let TIC = QualIQon.Plots.TICPlot.create

module createXICPlot = 
    let XIC = QualIQon.Plots.XICPlot.createXIC

module createMS1Map = 
    let MS1MapPlot = QualIQon.Plots.MS1MapPlot.createMS1Map