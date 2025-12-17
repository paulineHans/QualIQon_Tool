namespace QualIQon.Tools.CorrRatio

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open QualIQon.IO
open QualIQon.Plots

module createCorrRatioPlot =     
    let HeatmapCorrelationRatio = QualIQon.Plots.CorrRatioPlot.quantRatio   

    