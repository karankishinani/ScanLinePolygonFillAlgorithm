Imports System
Imports System.Collections

Public Class Principal
    Shared Sub Main()

        x_points.add(2)
        x_points.add(7)
        x_points.add(13)
        x_points.add(13)
        x_points.add(7)
        x_points.add(2)

        y_points.add(3)
        y_points.add(1)
        y_points.add(5)
        y_points.add(11)
        y_points.add(7)
        y_points.add(9)

        fillTA(TA)
        ' printTA(TA)
        polyfill(TA)
        Console.Read()
    End Sub
End Class

Public Class TA_node
    Public y_max As Integer
    Public x_min As Single
    Public m_numerator As Integer
    Public m_denominator As Integer
    Public y_min As Integer
    Public nextTA As TA_node
    'Constructor
    Sub New(ByVal _y_max As Integer, _x_min As Integer, _m_numerator As Integer, _m_denominator As Integer, _y_min As Integer)
        y_max = _y_max
        y_min = _y_min
        x_min = _x_min
        m_numerator = _m_numerator
        m_denominator = _m_denominator
        nextTA = Nothing
    End Sub

    Function inverse_m() As Single
        Return Convert.ToSingle(m_denominator) / m_numerator
    End Function
End Class

Class TA_list
    Dim list As New ArrayList
    ' Constructor
    Sub New()
        Dim i As Integer
        For i = 0 To 12 - 1
            list.add(Nothing)
        Next
    End Sub

    Sub add(ByVal scanline As Integer, ByRef node As TA_node)
        Dim actualnode As TA_node
        If list(scanline) Is Nothing Then
            list(scanline) = node
        Else
            actualnode = list(scanline)
            While Not actualnode.nextTA Is Nothing
                actualnode = actualnode.nextTA
            End While
            actualnode.nextTA = node
            node.nextTA = Nothing
        End If
    End Sub

    Sub delete(ByVal scanline As Integer, ByRef node As TA_node)
        Dim prevnode As TA_node = Nothing
        Dim currnode As TA_node = list(scanline)

        If Not currnode Is Nothing Then 'if the node is not null
            While Not currnode.nextTA Is Nothing 'while currnode is not the last node
                If currnode Is node Then
                    If prevnode Is Nothing Then ' if currnode is first
                        If Not currnode.nextTA Is Nothing Then 'if the first node is not last node
                            list(scanline) = currnode.nextTA
                        End If
                    Else 'if is a middle node
                        prevnode.nextTA = currnode.nextTA
                    End If
                End If
                prevnode = currnode
                currnode = currnode.nextTA
            End While
            'when its last node
            If currnode Is node And currnode.nextTA Is Nothing Then
                If prevnode Is Nothing Then 'if it is the first node
                    list(scanline) = Nothing
                Else
                    prevnode.nextTA = Nothing
                End If
            End If
        End If
    End Sub

    Function first_y() As Integer
        Dim i As Integer
        Dim item As TA_Node
        For i = 0 To list.Count - 1
            item = list(i)
            If Not item Is Nothing Then
                Return i
            End If
        Next
        Return 0
    End Function

    Function isEmpty() As Boolean
        Dim item As TA_Node
        For Each item In list
            If Not item Is Nothing Then
                Return False
            End If
        Next
        Return True
    End Function

    Function getList() As Arraylist
        Return list
    End Function

    Sub printScanLine(ByVal scanline As Integer)
        Dim node As TA_Node = list(scanline)
        If Not node Is Nothing Then
            While Not node.nextTA Is Nothing
                Console.Write(node.y_max & node.x_min & node.inverse_m & node.y_min & " | ")
                node = node.nextTA
            End While
            Console.Write(node.y_max & node.x_min & node.inverse_m & node.y_min & " | ")
        Else
            Console.Write("Nothing")
        End If
        Console.Write(vbCrLf)
    End Sub
End Class

Module ModTA
    Public x_points As New ArrayList
    Public y_points As New ArrayList
    Public TA As New TA_list()

    Sub fillTA(ByRef TA As TA_List)
        ' for each line in scanline
        Dim scanline As Integer
        Dim i As Integer
        Dim k As Integer
        For scanline = 0 To 12 - 1
            ' if the y coordinate of the edge is on the scanline
            For i = 0 To y_points.Count - 1
                If scanline = y_points(i) Then
                    'find connecting point on the left
                    k = modulo((i - 1), y_points.Count)
                    ' if the y coordinate of that point is greater
                    If y_points(k) > y_points(i) Then
                        ' Add the node with max(current-edge-y, left-connecting-y), current-edge-x, slope numerator and denominator in that scanline
                        TA.add(scanline, New TA_node(Math.Max(y_points(i), y_points(k)), x_points(i), y_points(i) - y_points(k), x_points(i) - x_points(k), Math.Min(y_points(i), y_points(k))))
                    End If

                    'find connecting point on the right
                    k = modulo((i + 1), y_points.Count)
                    ' if the y coordinate of that point is greater
                    If y_points(k) > y_points(i) Then
                        ' Add the node with max(current-edge-y, right-connecting-y), current-edge-x, slope numerator and denominator in that scanline
                        TA.add(scanline, New TA_node(Math.Max(y_points(i), y_points(k)), x_points(i), y_points(i) - y_points(k), x_points(i) - x_points(k), Math.Min(y_points(i), y_points(k))))
                    End If
                End If
            Next
        Next
    End Sub

    Sub bubblesort(ByRef alist As ArrayList)
        Dim passnum As Integer
        Dim i As Integer
        Dim temp As TA_node
        For passnum = alist.Count - 1 To 0 Step -1
            For i = 0 To passnum - 1
                If alist(i).x_min > alist(i + 1).x_min Then
                    temp = alist(i)
                    alist(i) = alist(i + 1)
                    alist(i + 1) = temp
                End If
            Next
        Next
    End Sub

    Public Function modulo(ByVal x As Integer, ByVal y As Integer) As Integer
        Return x - y * Math.Floor(x / y)

    End Function

    Sub polyfill(ByRef TA As TA_List)
        Dim y As Integer
        Dim lista_TA As New ArrayList
        Dim i As Integer
        Dim item As TA_node
        Dim node As TA_node
        '1. Poner y al valor mas pequeño de la coordenada y que esté en la TA (primera cubeta no vacía)
        y = TA.first_y()

        '2. Inicializar la TAA a vacío
        Dim TAA As New ArrayList

        '3. Reperir hasta que la TAA y TA estén vacios
        While Not (TAA.Count = 0 And TA.isEmpty())
            'a) Mover de la cubeta TA y a la TAA aquellas aristas cuya y_min = y (aristas de entrada)
            lista_TA = TA.getList()
            For i = y To lista_TA.Count - 1
                item = lista_TA(i)
                If Not item Is Nothing Then
                    node = item
                    While Not node.nextTA Is Nothing
                        If node.y_min = y Then
                            TAA.Add(node)
                            TA.delete(i, node)
                        End If
                        node = node.nextTA
                    End While
                    If node.y_min = y Then
                        TAA.Add(node)
                        TA.delete(i, node)
                    End If
                End If
            Next

            ' b) Quitar de la TAA aquellas entradas para las cuales y = y_max (las aristas no involucrados 
            '    en la siguiente línea de escaneo), entonces se ordena la TAA en x
            Dim removeList As New ArrayList
            For Each node In TAA
                If y = node.y_max Then
                    removeList.Add(node)
                End If
            Next

            For Each node In removeList
                TAA.Remove(node)
            Next

            ' ordenamiento con burbuja
            bubblesort(TAA)

            ' c) Rellenar los pixeles deseados sobre la línea de escaneo y usando pares de coordenadas x de la TAA
            i = 0
            For Each node In TAA
                If i Mod 2 = 0 Then
                    Console.Write("(" & (Math.Ceiling(node.x_min)) & ", " & y & ") ")
                Else
                    Console.Write("(" & (Math.Floor(node.x_min)) & ", " & y & ") ")
                End If
                i = i + 1
            Next
            Console.Write(vbCrLf)

            ' d) Incrementa y en 1 (siguiente linea de escaneo)
            y = y + 1

            ' e) Para cada arista no vertical que puede en la TAA, poner al día x para la nueva y
            For Each node In TAA
                node.x_min = node.x_min + node.inverse_m()
            Next
        End While
    End Sub

    Sub printTA(ByRef TA As TA_List)
        Console.Write(" --- TA ---")
        Dim i As Integer
        For i = 0 To TA.getList().Count - 1
            TA.printScanLine(i)
        Next
    End Sub

End Module


