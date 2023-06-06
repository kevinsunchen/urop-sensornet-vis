using System;
using UnityEngine;

// I would use C# 9.0's record struct feature for this but it seems like Unity doesn't fully support that yet
public class Point
{
    public int x;
    public int y;

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Point(int[] coords) {
        if (coords.Length != 2) {
            throw new ArgumentException("Parameter must be a length-2 array");
        }
        this.x = coords[0];
        this.y = coords[1];
    }

    public override string ToString() {
        return $"({x},{y})";
    }
    
    public override bool Equals(System.Object obj) {
        if ((obj == null) || ! this.GetType().Equals(obj.GetType())) {
            return false;
        } else {
            Point p = (Point) obj;
            return (x == p.x) && (y == p.y);
        }
    }

    public override int GetHashCode() {
        return x ^ y;
    }
}
