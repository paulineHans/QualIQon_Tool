The QualIQon Tool 
------------------

QualIQon is a tool for visual quality control of standardized data analysis workflows in mass spectrometry-based proteomics. 

This is achieved through the visualization of quality metrics derived from key steps in DDA-MS proteomics workflows, such as peptide spectrum matching, quantification, and protein identification. 

The tool supports widely used analysis pipelines including ProteomIQon (https://github.com/CSBiology/ProteomIQon), MaxQuant, and FragPipe. 

The Core Project 
----------------- 
The aim of QualIQon is to enable automated and reproducible visual quality assessment of proteomics data, within a containerised Docker environment, embedded in the ARC framework (https://www.nfdi4plants.org/), and fully compliant with FAIR principles. The visual analytics include various plots assessing proteolytic digestion, score refinement, local correlation values, protein identification and more. 

Documentation 
-------------- 

No documentation is available at the moment, but it is currently being worked on. If you have any questions or problems while using the tool, please contact me by email (phans@rptu.de) or via my Github profile. 

Contributing
-----------------

Please refer to the CSB [Contributing guidelines](.github/CONTRIBUTING.md)

Plots you get from QualIQon
----------------------------

1. Misscleavages (MCs) Plot 
Shows the relative Distribution of MCs in all MS-runs. Currently set to max. two MCs 

2. XIC 
Extracted Ion Chromatogram as 3D Plot which includes all MS-runs

3. TIC 
Total Ion Chromatogram as 3D Plot which includes all MS-runs

4. MS1Map 

5. Protein Identification 

6. Score Refinement Plot (ProteomIQon specific)

7. Visualization of the Correlation Light Heavy Parameter (ProteomIQon specific)

8. Heatmap - Correlation of 14N data 

9. Heatmap - Correlation of 15N data 

10. Heatmap - Correlation of 14N/15N data 

