namespace QualIQon.Tool.CorrHeavy

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open ProteomIQon

module createCorrHeavyPlot =
    let HeatmapCorrelationHeavy = QualIQon.Plots.CorrHeavyPlot.quantHeavy
