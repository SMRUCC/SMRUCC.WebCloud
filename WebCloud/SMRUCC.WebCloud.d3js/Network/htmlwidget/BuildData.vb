﻿#Region "Microsoft.VisualBasic::d76fb7c5e574d9a3c8e2443a562a330e, ..\httpd\WebCloud\SMRUCC.WebCloud.d3js\Network\htmlwidget\BuildData.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports NetGraphData = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkTables

Namespace Network.htmlwidget

    ''' <summary>
    ''' 将``htmlwidget``之中的D3.js网络模型解析为scibasic的标准网络模型
    ''' </summary>
    Public Module BuildData

        Const JSON$ = "<script type[=]""application/json"".+?</script>"

        ''' <summary>
        ''' 参数为html文本或者url路径
        ''' </summary>
        ''' <param name="html$"></param>
        ''' <returns></returns>
        Public Function ParseHTML(html$) As String
            If html.FileExists Then
                html = html.GET
            End If

            html = Regex.Match(html, BuildData.JSON, RegexICSng).Value
            html = html.GetStackValue(">", "<")

            Return html
        End Function

        Public Function BuildGraph(html$) As NetGraphData
            Dim json$ = BuildData.ParseHTML(html)
            Dim data As htmlwidget.NetGraph = json.LoadObject(Of htmlwidget.JSON).x
            Dim nodes As New List(Of Node)
            Dim edges As New List(Of NetworkEdge)

            For i As Integer = 0 To data.nodes.name.Length - 1
                Dim name$ = data.nodes.name(i)
                Dim type$ = data.nodes.group(i)

                nodes += New Node With {
                    .ID = name,
                    .NodeType = type
                }
            Next

            Dim nodesVector As Node() = nodes.ToArray

            For i As Integer = 0 To data.links.source.Length - 1
                Dim src = nodesVector(data.links.source(i)).ID
                Dim tar = nodesVector(data.links.target(i)).ID
                Dim type = data.links.colour(i)

                edges += New NetworkEdge With {
                    .FromNode = src, 
                    .ToNode = tar, 
                    .value = 1, 
                    .Interaction = type
                }
            Next

            Dim net As New NetGraphData With {
                .Nodes = nodes,
                .Edges = edges
            }
            Return net
        End Function
    End Module
End Namespace
