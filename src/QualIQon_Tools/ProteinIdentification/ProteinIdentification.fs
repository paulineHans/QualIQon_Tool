namespace QualIQon.Tools.ProteinId

open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open ProteomIQon.Dto
open Plotly.NET
open System
open BioFSharp
open BioFSharp.IO
open BioFSharp.PeptideClassification
open QualIQon.IO
open QualIQon.Plots

module createProteinIdentificationPlot = 
    let ProteinIdentification = QualIQon.Plots.ProteinIdentificationPlot.PI