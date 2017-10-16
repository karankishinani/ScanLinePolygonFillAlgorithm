# -*- coding: utf-8 -*-
import math
class TA_list(object):
    
    def __init__(self):
        self.list = []
        for i in range(0, 12):
            self.list.append(None)
    
    def add(self, scanline, node):
        if self.list[scanline] == None:
            self.list[scanline] = node
        else:
            actualnode = self.list[scanline]
            while actualnode.nextTA != None:
                actualnode = actualnode.nextTA
            actualnode.nextTA = node
            node.nextTA = None
            
    def delete(self, scanline, node):
        prevnode = None
        currnode = self.list[scanline]

        if currnode != None: #if the node is not null
            while currnode.nextTA != None: #while currnode is not the last node
                if currnode == node:
                    if prevnode == None: # if currnode is first
                        if currnode.nextTA != None: #if the first node is not last node
                            self.list[scanline] = currnode.nextTA
                    else: #if is a middle node
                        prevnode.nextTA = currnode.nextTA
                prevnode = currnode
                currnode = currnode.nextTA
                
            #when its last node
            if currnode == node and currnode.nextTA == None:
                if prevnode == None: #if it is the first node
                    self.list[scanline] = None
                else:
                    prevnode.nextTA = None
                
    def first_y(self):
        for i in range(0,len(self.list)):
            item = self.list[i]
            if item != None:
                return i
            
    def isEmpty(self):
        for item in self.list:
            if item != None:
                return False
        return True
        
    def getList(self):
        return self.list
            
    def printScanline(self, scanline):
        node = self.list[scanline]
        if node != None:
            while node.nextTA != None:
                print node.y_max, node.x_min,  node.inverse_m(), node.y_min, ' | ',
                node = node.nextTA
            print node.y_max, node.x_min,  node.inverse_m(), node.y_min, ' | ',
        else:
            print 'None',
        print

class TA_node(object):
    
    def __init__(self, y_max, x_min, m_numerator, m_denominator, y_min):
        self.y_max = y_max
        self.y_min = y_min
        self.x_min = x_min
        self.m_numerator = m_numerator
        self.m_denominator = m_denominator
        self.nextTA = None
        
    def inverse_m(self):
        return float(self.m_denominator) / self.m_numerator

def fillTA(TA):
    # for each line in scanline
    for scanline in range(0, 12):
        # if the y coordinate of the edge is on the scanline
        for i in range(0,len(y_points)):
            if scanline == y_points[i]:
                
                # find connecting point on the left
                k = (i - 1) % len(y_points)
                # if the y coordinate of that point is greater
                if y_points[k] > y_points[i]:
                    # Add the node with max(current-edge-y, left-connecting-y), current-edge-x, slope numerator and denominator in that scanline
                    TA.add(scanline, TA_node(max(y_points[i], y_points[k]), x_points[i], y_points[i]-y_points[k], x_points[i]-x_points[k], min(y_points[i], y_points[k])))
                    
                # find connecting point on the right
                k = (i + 1) % len(y_points)
    
                # if the y coordinate of that point is greater
                if y_points[k] > y_points[i]:
                    # Add the node with max(current-edge-y, right-connecting-y), current-edge-x, slope numerator and denominator in that scanline
                    TA.add(scanline, TA_node(max(y_points[i], y_points[k]), x_points[i], y_points[i]-y_points[k], x_points[i]-x_points[k], min(y_points[i], y_points[k])))
            
def bubblesort(alist):
    for passnum in range(len(alist)-1,0,-1):
        for i in range(passnum):
            if alist[i].x_min>alist[i+1].x_min:
                temp = alist[i]
                alist[i] = alist[i+1]
                alist[i+1] = temp
                   
def polyfill(TA):
    
    # 1. Poner y al valor mas pequeño de la coordenada y que esté en la TA (primera cubeta no vacía)
    y = TA.first_y()
    
    # 2. Inicilizar la TAA a vacío
    TAA = []
    
    # 3. Repetir hasta que la TAA y TA estén vacios
    while not(len(TAA) == 0 and TA.isEmpty()):
        # a) Mover de la cubeta TA y a la TAA aquellas aristas cuya y_min = y (aristas de entrada)
        lista_TA = TA.getList()
        for i in range(y, len(lista_TA)):
            item = lista_TA[i]
            if item != None:
                node = item
                while node.nextTA != None:
                    if node.y_min == y:
                        TAA.append(node)
                        TA.delete(i,node)
                    node = node.nextTA
                if node.y_min == y:
                    TAA.append(node)
                    TA.delete(i,node)
    
            #printTA()
                    
            # b) Quitar de la TAA aquellas entradas para las cuales y = y_max (las aristas no involucrados 
            #    en la siguiente línea de escaneo), entonces se ordena la TAA en x
            removeList = []
            for node in TAA:
                if y == node.y_max:
                    removeList.append(node)
                    
            for node in removeList:
                TAA.remove(node)
                    
            # ordenamiento con Burbuja
            bubblesort(TAA)
            
            # c) Rellenar los pixeles deseados sobre la línea de escaneo y usando pares de coordenadas x de la TAA
            i = 0
            for node in TAA:
                if i % 2 == 0:
                    print '(',int(math.ceil(node.x_min)),',',y,') ',
                else:
                    print '(',int(math.floor(node.x_min)),',',y,') ',
                i = i + 1
            print
            
            # d) Incrementa y en 1 (siguiente linea de escaneo)
            y = y + 1
            # e) Para cada arista no vertical que quede en la TAA, poner al día x para la nueva y
            for node in TAA:
                node.x_min = node.x_min + node.inverse_m()
        
def printTA():
    print ' --- TA ---'
    for i in range(0, len(TA.list)):
        TA.printScanline(i)

x_points = [2, 7, 13, 13, 7, 2]
y_points = [3, 1, 5, 11, 7, 9]        
TA = TA_list()
fillTA(TA)
#printTA()
polyfill(TA)