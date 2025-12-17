namespace QualIQon.Tools.XIC

open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model
open QualIQon.IO
open QualIQon.Plots


module createXICPlot = 
    let XIC = QualIQon.Plots.XICPlot.createXIC