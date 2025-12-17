namespace QualIQon.Tools.ScoreRefinement

open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Plotly.NET
open ProteomIQon.Dto
open QualIQon.IO
open QualIQon.Plots

module createScoreRefinementPlot = 
    let ScoreRefinement = QualIQon.Plots.ScoreRefinementPlot.SR