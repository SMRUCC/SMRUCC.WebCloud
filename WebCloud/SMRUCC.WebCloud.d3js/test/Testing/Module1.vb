﻿#Region "Microsoft.VisualBasic::f26cd786d483d207ec933ae3705be493, ..\httpd\WebCloud\SMRUCC.WebCloud.d3js\test\Testing\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.ConvexHull
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Mathematical.Interpolation
Imports names = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic.NameOf

Module Module1

    Sub Main()
        Dim json = SMRUCC.WebCloud.d3js.Network.htmlwidget.BuildData.BuildGraph("../../..\viewer.html")
        Dim colors As SolidBrush() = Designer.GetColors("d3.scale.category10()") _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray ' using d3.js colors
        Dim graph = json.CreateGraph(Function(n) colors(CInt(n.NodeType)))
        Dim nodePoints As Dictionary(Of Graph.Node, Point) = Nothing

        NetworkVisualizer.DefaultEdgeColor = Color.DimGray

        Call graph.doRandomLayout
        Call graph.doForceLayout(showProgress:=True, Repulsion:=2000, Stiffness:=40, Damping:=0.5, iterations:=500)
        Call graph.Tabular.Save("./test")

        Dim canvas As Image = graph _
            .DrawImage(
                canvasSize:="2000,2000",
                scale:=4,
                labelColorAsNodeColor:=True,
                nodePoints:=nodePoints) _
            .AsGDIImage

        Dim groups = graph.nodes _
            .GroupBy(Function(x)
                         Return x.Data.Properties(names.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function) _
            .ToArray

        Using g As Graphics2D = canvas.CreateCanvas2D(directAccess:=True)
            For Each group In groups
                Dim polygon As Point() = group _
                    .Select(Function(node) nodePoints(node)) _
                    .ToArray

                ' 只有两个点或者一个点是无法计算凸包的，则跳过这些点
                If polygon.Length <= 3 Then
                    Continue For
                End If

                ' 凸包算法计算出边界
                polygon = ConvexHull.JarvisMatch(polygon).Enlarge(1.5)

                '' 凸包计算出来的多边形进行矢量放大1.5倍，并进行二次插值处理
                'With polygon.Enlarge(scale:=2).AsList
                '    Call .Add(.First)

                '    polygon = .BSpline(2, RESOLUTION:=3).ToPoints
                'End With

                ' 描绘出边界
                Dim gcolor As SolidBrush = colors(CInt(group.Key))
                Dim transparent = Color.FromArgb(30, gcolor.Color)

                Call g.DrawPolygon(New Pen(gcolor, 3), polygon)
                Call g.FillPolygon(New SolidBrush(transparent), polygon)
            Next
        End Using

        Call canvas.SaveAs("../../..\/viewer.png")
    End Sub
End Module

