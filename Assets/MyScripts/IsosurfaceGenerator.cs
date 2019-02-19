using System.Collections.Generic;

public class IsosurfaceGenerator{

    private static int[][] EdgeDirAndStart = 
        {
            new int[] {0,  0, 0, 0},  //  0 (0 -x-> 1)
            new int[] {1,  0, 0, 0},  //  1 (0 -y-> 2)
            new int[] {1,  1, 0, 0},  //  2 (1 -y-> 3)
            new int[] {0,  0, 1, 0},  //  3 (2 -x-> 3)
            new int[] {0,  0, 0, 1},  //  4 (4 -x-> 5)
            new int[] {1,  0, 0, 1},  //  5 (4 -y-> 6)
            new int[] {1,  1, 0, 1},  //  6 (5 -y-> 7)
            new int[] {0,  0, 1, 1},  //  7 (6 -x-> 7)
            new int[] {2,  0, 0, 0},  //  8 (0 -z-> 4)
            new int[] {2,  1, 0, 0},  //  9 (1 -z-> 5)
            new int[] {2,  0, 1, 0},  // 10 (2 -z-> 6)
            new int[] {2,  1, 1, 0}   // 11 (3 -z-> 7)
        };
    
    private static List<int[]>[] PlanePattern =
        {
            new List<int[]> {  },
            new List<int[]> { new int[]{ 7,11, 6},  },
            new List<int[]> { new int[]{10, 7, 5},  },
            new List<int[]> { new int[]{ 5,10, 6}, new int[]{10,11, 6},  },
            new List<int[]> { new int[]{ 9, 4, 6},  },
            new List<int[]> { new int[]{ 9, 4,11}, new int[]{ 4, 7,11},  },
            new List<int[]> { new int[]{ 6, 9, 4}, new int[]{ 7, 5,10},  },
            new List<int[]> { new int[]{ 5,10, 4}, new int[]{ 4,10, 9}, new int[]{10,11, 9},  },
            new List<int[]> { new int[]{ 8, 5, 4},  },
            new List<int[]> { new int[]{ 7,11, 6}, new int[]{ 5, 4, 8},  },
            new List<int[]> { new int[]{ 4, 8, 7}, new int[]{ 8,10, 7},  },
            new List<int[]> { new int[]{ 4, 8, 6}, new int[]{ 6, 8,11}, new int[]{ 8,10,11},  },
            new List<int[]> { new int[]{ 6, 9, 5}, new int[]{ 9, 8, 5},  },
            new List<int[]> { new int[]{ 7,11, 5}, new int[]{ 5,11, 8}, new int[]{11, 9, 8},  },
            new List<int[]> { new int[]{ 6, 9, 7}, new int[]{ 7, 9,10}, new int[]{ 9, 8,10},  },
            new List<int[]> { new int[]{10,11, 9}, new int[]{10, 9, 8},  },
            new List<int[]> { new int[]{11, 3, 2},  },
            new List<int[]> { new int[]{ 3, 2, 7}, new int[]{ 2, 6, 7},  },
            new List<int[]> { new int[]{ 7, 5,10}, new int[]{11, 3, 2},  },
            new List<int[]> { new int[]{ 3, 2,10}, new int[]{10, 2, 5}, new int[]{ 2, 6, 5},  },
            new List<int[]> { new int[]{ 9, 4, 6}, new int[]{ 2,11, 3},  },
            new List<int[]> { new int[]{ 9, 4, 2}, new int[]{ 2, 4, 3}, new int[]{ 4, 7, 3},  },
            new List<int[]> { new int[]{ 2,11, 3}, new int[]{ 9, 4, 6}, new int[]{10, 7, 5},  },
            new List<int[]> { new int[]{ 5, 9, 4}, new int[]{10, 9, 5}, new int[]{10, 2, 9}, new int[]{10, 3, 2},  },
            new List<int[]> { new int[]{ 8, 5, 4}, new int[]{ 3, 2,11},  },
            new List<int[]> { new int[]{ 3, 2, 7}, new int[]{ 2, 6, 7}, new int[]{ 5, 4, 8},  },
            new List<int[]> { new int[]{ 4, 8, 7}, new int[]{ 8,10, 7}, new int[]{11, 3, 2},  },
            new List<int[]> { new int[]{ 3, 8,10}, new int[]{ 3, 6, 8}, new int[]{ 3, 2, 6}, new int[]{ 6, 4, 8},  },
            new List<int[]> { new int[]{ 8, 5, 9}, new int[]{ 5, 6, 9}, new int[]{ 2,11, 3},  },
            new List<int[]> { new int[]{ 5, 9, 8}, new int[]{ 5, 3, 9}, new int[]{ 5, 7, 3}, new int[]{ 9, 3, 2},  },
            new List<int[]> { new int[]{ 6, 9, 7}, new int[]{ 7, 9,10}, new int[]{ 9, 8,10}, new int[]{11, 3, 2},  },
            new List<int[]> { new int[]{ 3, 2,10}, new int[]{ 2, 9,10}, new int[]{10, 9, 8},  },
            new List<int[]> { new int[]{ 3,10, 1},  },
            new List<int[]> { new int[]{10, 1, 3}, new int[]{ 7,11, 6},  },
            new List<int[]> { new int[]{ 7, 5, 3}, new int[]{ 5, 1, 3},  },
            new List<int[]> { new int[]{11, 6, 3}, new int[]{ 3, 6, 1}, new int[]{ 6, 5, 1},  },
            new List<int[]> { new int[]{ 3,10, 1}, new int[]{ 6, 9, 4},  },
            new List<int[]> { new int[]{ 9, 4,11}, new int[]{ 4, 7,11}, new int[]{ 3,10, 1},  },
            new List<int[]> { new int[]{ 1, 3, 5}, new int[]{ 3, 7, 5}, new int[]{ 4, 6, 9},  },
            new List<int[]> { new int[]{ 3, 5, 1}, new int[]{ 3, 9, 5}, new int[]{ 3,11, 9}, new int[]{ 5, 9, 4},  },
            new List<int[]> { new int[]{ 5, 4, 8}, new int[]{10, 1, 3},  },
            new List<int[]> { new int[]{ 6, 7,11}, new int[]{ 4, 8, 5}, new int[]{ 3,10, 1},  },
            new List<int[]> { new int[]{ 1, 3, 8}, new int[]{ 8, 3, 4}, new int[]{ 3, 7, 4},  },
            new List<int[]> { new int[]{ 1, 4, 8}, new int[]{ 3, 4, 1}, new int[]{ 3, 6, 4}, new int[]{ 3,11, 6},  },
            new List<int[]> { new int[]{ 6, 9, 5}, new int[]{ 9, 8, 5}, new int[]{10, 1, 3},  },
            new List<int[]> { new int[]{ 7,11, 5}, new int[]{ 5,11, 8}, new int[]{11, 9, 8}, new int[]{10, 1, 3},  },
            new List<int[]> { new int[]{ 1, 9, 8}, new int[]{ 1, 7, 9}, new int[]{ 1, 3, 7}, new int[]{ 7, 6, 9},  },
            new List<int[]> { new int[]{ 1, 3, 8}, new int[]{ 3,11, 8}, new int[]{ 8,11, 9},  },
            new List<int[]> { new int[]{10, 1,11}, new int[]{ 1, 2,11},  },
            new List<int[]> { new int[]{10, 1, 7}, new int[]{ 7, 1, 6}, new int[]{ 1, 2, 6},  },
            new List<int[]> { new int[]{ 7, 5,11}, new int[]{11, 5, 2}, new int[]{ 5, 1, 2},  },
            new List<int[]> { new int[]{ 1, 2, 6}, new int[]{ 1, 6, 5},  },
            new List<int[]> { new int[]{10, 1,11}, new int[]{ 1, 2,11}, new int[]{ 6, 9, 4},  },
            new List<int[]> { new int[]{10, 4, 7}, new int[]{10, 2, 4}, new int[]{10, 1, 2}, new int[]{ 2, 9, 4},  },
            new List<int[]> { new int[]{ 7, 5,11}, new int[]{11, 5, 2}, new int[]{ 5, 1, 2}, new int[]{ 6, 9, 4},  },
            new List<int[]> { new int[]{ 9, 4, 2}, new int[]{ 4, 5, 2}, new int[]{ 2, 5, 1},  },
            new List<int[]> { new int[]{ 2,11, 1}, new int[]{11,10, 1}, new int[]{ 8, 5, 4},  },
            new List<int[]> { new int[]{10, 1, 7}, new int[]{ 7, 1, 6}, new int[]{ 1, 2, 6}, new int[]{ 5, 4, 8},  },
            new List<int[]> { new int[]{ 8, 7, 4}, new int[]{ 8, 2, 7}, new int[]{ 8, 1, 2}, new int[]{ 7, 2,11},  },
            new List<int[]> { new int[]{ 4, 8, 6}, new int[]{ 8, 1, 6}, new int[]{ 6, 1, 2},  },
            new List<int[]> { new int[]{ 8, 5, 6}, new int[]{ 8, 6, 9}, new int[]{ 1,11,10}, new int[]{ 1, 2,11},  },
            new List<int[]> { new int[]{ 9, 8, 2}, new int[]{ 2, 8, 1}, new int[]{ 5, 7,10},  },
            new List<int[]> { new int[]{ 1, 2, 8}, new int[]{ 8, 2, 9}, new int[]{11, 7, 6},  },
            new List<int[]> { new int[]{ 1, 2, 8}, new int[]{ 8, 2, 9},  },
            new List<int[]> { new int[]{ 2, 0, 9},  },
            new List<int[]> { new int[]{ 2, 0, 9}, new int[]{11, 6, 7},  },
            new List<int[]> { new int[]{10, 7, 5}, new int[]{ 2, 0, 9},  },
            new List<int[]> { new int[]{ 5,10, 6}, new int[]{10,11, 6}, new int[]{ 9, 2, 0},  },
            new List<int[]> { new int[]{ 4, 6, 0}, new int[]{ 6, 2, 0},  },
            new List<int[]> { new int[]{ 2, 0,11}, new int[]{11, 0, 7}, new int[]{ 0, 4, 7},  },
            new List<int[]> { new int[]{ 2, 0, 6}, new int[]{ 0, 4, 6}, new int[]{ 7, 5,10},  },
            new List<int[]> { new int[]{ 5, 0, 4}, new int[]{ 5,11, 0}, new int[]{ 5,10,11}, new int[]{11, 2, 0},  },
            new List<int[]> { new int[]{ 8, 5, 4}, new int[]{ 0, 9, 2},  },
            new List<int[]> { new int[]{ 0, 9, 2}, new int[]{ 8, 5, 4}, new int[]{11, 6, 7},  },
            new List<int[]> { new int[]{10, 7, 8}, new int[]{ 7, 4, 8}, new int[]{ 0, 9, 2},  },
            new List<int[]> { new int[]{ 4, 8, 6}, new int[]{ 6, 8,11}, new int[]{ 8,10,11}, new int[]{ 9, 2, 0},  },
            new List<int[]> { new int[]{ 8, 5, 0}, new int[]{ 0, 5, 2}, new int[]{ 5, 6, 2},  },
            new List<int[]> { new int[]{ 8, 2, 0}, new int[]{ 5, 2, 8}, new int[]{ 5,11, 2}, new int[]{ 5, 7,11},  },
            new List<int[]> { new int[]{ 7, 8,10}, new int[]{ 7, 2, 8}, new int[]{ 7, 6, 2}, new int[]{ 8, 2, 0},  },
            new List<int[]> { new int[]{ 2, 0,11}, new int[]{ 0, 8,11}, new int[]{11, 8,10},  },
            new List<int[]> { new int[]{ 0, 9, 3}, new int[]{ 9,11, 3},  },
            new List<int[]> { new int[]{ 6, 7, 9}, new int[]{ 9, 7, 0}, new int[]{ 7, 3, 0},  },
            new List<int[]> { new int[]{ 0, 9, 3}, new int[]{ 9,11, 3}, new int[]{10, 7, 5},  },
            new List<int[]> { new int[]{10, 6, 5}, new int[]{10, 0, 6}, new int[]{10, 3, 0}, new int[]{ 6, 0, 9},  },
            new List<int[]> { new int[]{11, 3, 6}, new int[]{ 6, 3, 4}, new int[]{ 3, 0, 4},  },
            new List<int[]> { new int[]{ 7, 3, 0}, new int[]{ 7, 0, 4},  },
            new List<int[]> { new int[]{11, 3, 6}, new int[]{ 6, 3, 4}, new int[]{ 3, 0, 4}, new int[]{ 7, 5,10},  },
            new List<int[]> { new int[]{ 5,10, 4}, new int[]{10, 3, 4}, new int[]{ 4, 3, 0},  },
            new List<int[]> { new int[]{11, 3, 9}, new int[]{ 3, 0, 9}, new int[]{ 4, 8, 5},  },
            new List<int[]> { new int[]{ 6, 7, 9}, new int[]{ 9, 7, 0}, new int[]{ 7, 3, 0}, new int[]{ 4, 8, 5},  },
            new List<int[]> { new int[]{10, 7, 4}, new int[]{10, 4, 8}, new int[]{ 3, 9,11}, new int[]{ 3, 0, 9},  },
            new List<int[]> { new int[]{ 3, 0,10}, new int[]{10, 0, 8}, new int[]{ 9, 6, 4},  },
            new List<int[]> { new int[]{11, 5, 6}, new int[]{11, 0, 5}, new int[]{11, 3, 0}, new int[]{ 0, 8, 5},  },
            new List<int[]> { new int[]{ 8, 5, 0}, new int[]{ 5, 7, 0}, new int[]{ 0, 7, 3},  },
            new List<int[]> { new int[]{ 8,10, 0}, new int[]{ 0,10, 3}, new int[]{ 7, 6,11},  },
            new List<int[]> { new int[]{ 8,10, 0}, new int[]{ 0,10, 3},  },
            new List<int[]> { new int[]{ 3,10, 1}, new int[]{ 2, 0, 9},  },
            new List<int[]> { new int[]{ 1, 3,10}, new int[]{ 0, 9, 2}, new int[]{ 7,11, 6},  },
            new List<int[]> { new int[]{ 7, 5, 3}, new int[]{ 5, 1, 3}, new int[]{ 2, 0, 9},  },
            new List<int[]> { new int[]{11, 6, 3}, new int[]{ 3, 6, 1}, new int[]{ 6, 5, 1}, new int[]{ 2, 0, 9},  },
            new List<int[]> { new int[]{ 4, 6, 0}, new int[]{ 6, 2, 0}, new int[]{ 1, 3,10},  },
            new List<int[]> { new int[]{ 2, 0,11}, new int[]{11, 0, 7}, new int[]{ 0, 4, 7}, new int[]{ 3,10, 1},  },
            new List<int[]> { new int[]{ 2, 0, 4}, new int[]{ 2, 4, 6}, new int[]{ 3, 5, 1}, new int[]{ 3, 7, 5},  },
            new List<int[]> { new int[]{ 5, 1, 4}, new int[]{ 4, 1, 0}, new int[]{ 3,11, 2},  },
            new List<int[]> { new int[]{10, 1, 3}, new int[]{ 5, 4, 8}, new int[]{ 2, 0, 9},  },
            new List<int[]> { new int[]{ 3,10, 1}, new int[]{11, 6, 7}, new int[]{ 2, 0, 9}, new int[]{ 5, 4, 8},  },
            new List<int[]> { new int[]{ 1, 3, 8}, new int[]{ 8, 3, 4}, new int[]{ 3, 7, 4}, new int[]{ 0, 9, 2},  },
            new List<int[]> { new int[]{ 3,11, 2}, new int[]{ 1, 0, 8}, new int[]{ 6, 4, 9},  },
            new List<int[]> { new int[]{ 8, 5, 0}, new int[]{ 0, 5, 2}, new int[]{ 5, 6, 2}, new int[]{ 1, 3,10},  },
            new List<int[]> { new int[]{ 5, 7,10}, new int[]{ 8, 1, 0}, new int[]{11, 2, 3},  },
            new List<int[]> { new int[]{ 6, 2, 7}, new int[]{ 7, 2, 3}, new int[]{ 0, 8, 1},  },
            new List<int[]> { new int[]{ 1, 0, 8}, new int[]{ 3,11, 2},  },
            new List<int[]> { new int[]{ 0, 9, 1}, new int[]{ 1, 9,10}, new int[]{ 9,11,10},  },
            new List<int[]> { new int[]{ 6, 0, 9}, new int[]{ 7, 0, 6}, new int[]{ 7, 1, 0}, new int[]{ 7,10, 1},  },
            new List<int[]> { new int[]{ 0, 5, 1}, new int[]{ 0,11, 5}, new int[]{ 0, 9,11}, new int[]{11, 7, 5},  },
            new List<int[]> { new int[]{ 0, 9, 1}, new int[]{ 9, 6, 1}, new int[]{ 1, 6, 5},  },
            new List<int[]> { new int[]{ 6, 0, 4}, new int[]{ 6,10, 0}, new int[]{ 6,11,10}, new int[]{ 0,10, 1},  },
            new List<int[]> { new int[]{10, 1, 7}, new int[]{ 1, 0, 7}, new int[]{ 7, 0, 4},  },
            new List<int[]> { new int[]{ 0, 4, 1}, new int[]{ 1, 4, 5}, new int[]{ 6,11, 7},  },
            new List<int[]> { new int[]{ 5, 1, 4}, new int[]{ 4, 1, 0},  },
            new List<int[]> { new int[]{ 0, 9, 1}, new int[]{ 1, 9,10}, new int[]{ 9,11,10}, new int[]{ 8, 5, 4},  },
            new List<int[]> { new int[]{ 7,10, 5}, new int[]{ 6, 4, 9}, new int[]{ 1, 0, 8},  },
            new List<int[]> { new int[]{ 7, 4,11}, new int[]{11, 4, 9}, new int[]{ 8, 1, 0},  },
            new List<int[]> { new int[]{ 0, 8, 1}, new int[]{ 9, 6, 4},  },
            new List<int[]> { new int[]{11,10, 6}, new int[]{ 6,10, 5}, new int[]{ 1, 0, 8},  },
            new List<int[]> { new int[]{10, 5, 7}, new int[]{ 1, 0, 8},  },
            new List<int[]> { new int[]{ 0, 8, 1}, new int[]{11, 7, 6},  },
            new List<int[]> { new int[]{ 0, 8, 1},  },
            new List<int[]> { new int[]{ 0, 1, 8},  },
            new List<int[]> { new int[]{ 0, 1, 8}, new int[]{11, 6, 7},  },
            new List<int[]> { new int[]{10, 7, 5}, new int[]{ 1, 8, 0},  },
            new List<int[]> { new int[]{11, 6,10}, new int[]{ 6, 5,10}, new int[]{ 1, 8, 0},  },
            new List<int[]> { new int[]{ 0, 1, 8}, new int[]{ 9, 4, 6},  },
            new List<int[]> { new int[]{ 7,11, 4}, new int[]{11, 9, 4}, new int[]{ 8, 0, 1},  },
            new List<int[]> { new int[]{ 7, 5,10}, new int[]{ 6, 9, 4}, new int[]{ 1, 8, 0},  },
            new List<int[]> { new int[]{ 5,10, 4}, new int[]{ 4,10, 9}, new int[]{10,11, 9}, new int[]{ 8, 0, 1},  },
            new List<int[]> { new int[]{ 5, 4, 1}, new int[]{ 4, 0, 1},  },
            new List<int[]> { new int[]{ 0, 1, 4}, new int[]{ 1, 5, 4}, new int[]{ 6, 7,11},  },
            new List<int[]> { new int[]{10, 7, 1}, new int[]{ 1, 7, 0}, new int[]{ 7, 4, 0},  },
            new List<int[]> { new int[]{ 1, 4, 0}, new int[]{ 1,11, 4}, new int[]{ 1,10,11}, new int[]{ 4,11, 6},  },
            new List<int[]> { new int[]{ 0, 1, 9}, new int[]{ 9, 1, 6}, new int[]{ 1, 5, 6},  },
            new List<int[]> { new int[]{ 7, 1, 5}, new int[]{ 7, 9, 1}, new int[]{ 7,11, 9}, new int[]{ 9, 0, 1},  },
            new List<int[]> { new int[]{ 0, 6, 9}, new int[]{ 1, 6, 0}, new int[]{ 1, 7, 6}, new int[]{ 1,10, 7},  },
            new List<int[]> { new int[]{ 0, 1, 9}, new int[]{ 1,10, 9}, new int[]{ 9,10,11},  },
            new List<int[]> { new int[]{ 1, 8, 0}, new int[]{ 3, 2,11},  },
            new List<int[]> { new int[]{ 6, 7, 2}, new int[]{ 7, 3, 2}, new int[]{ 0, 1, 8},  },
            new List<int[]> { new int[]{ 5,10, 7}, new int[]{ 8, 0, 1}, new int[]{11, 3, 2},  },
            new List<int[]> { new int[]{ 3, 2,10}, new int[]{10, 2, 5}, new int[]{ 2, 6, 5}, new int[]{ 1, 8, 0},  },
            new List<int[]> { new int[]{ 3, 2,11}, new int[]{ 1, 8, 0}, new int[]{ 6, 9, 4},  },
            new List<int[]> { new int[]{ 9, 4, 2}, new int[]{ 2, 4, 3}, new int[]{ 4, 7, 3}, new int[]{ 0, 1, 8},  },
            new List<int[]> { new int[]{ 0, 1, 8}, new int[]{ 2,11, 3}, new int[]{ 9, 4, 6}, new int[]{10, 7, 5},  },
            new List<int[]> { new int[]{10, 3, 1}, new int[]{ 5, 8, 4}, new int[]{ 2, 9, 0},  },
            new List<int[]> { new int[]{ 5, 4, 1}, new int[]{ 4, 0, 1}, new int[]{ 3, 2,11},  },
            new List<int[]> { new int[]{ 0, 1, 5}, new int[]{ 0, 5, 4}, new int[]{ 2, 7, 3}, new int[]{ 2, 6, 7},  },
            new List<int[]> { new int[]{10, 7, 1}, new int[]{ 1, 7, 0}, new int[]{ 7, 4, 0}, new int[]{ 3, 2,11},  },
            new List<int[]> { new int[]{ 4, 0, 6}, new int[]{ 6, 0, 2}, new int[]{ 1,10, 3},  },
            new List<int[]> { new int[]{ 0, 1, 9}, new int[]{ 9, 1, 6}, new int[]{ 1, 5, 6}, new int[]{ 2,11, 3},  },
            new List<int[]> { new int[]{ 7, 3, 5}, new int[]{ 5, 3, 1}, new int[]{ 2, 9, 0},  },
            new List<int[]> { new int[]{ 1,10, 3}, new int[]{ 0, 2, 9}, new int[]{ 7, 6,11},  },
            new List<int[]> { new int[]{ 3, 1,10}, new int[]{ 2, 9, 0},  },
            new List<int[]> { new int[]{ 8, 0,10}, new int[]{ 0, 3,10},  },
            new List<int[]> { new int[]{ 8, 0,10}, new int[]{ 0, 3,10}, new int[]{ 7,11, 6},  },
            new List<int[]> { new int[]{ 8, 0, 5}, new int[]{ 5, 0, 7}, new int[]{ 0, 3, 7},  },
            new List<int[]> { new int[]{ 8, 6, 5}, new int[]{ 8, 3, 6}, new int[]{ 8, 0, 3}, new int[]{ 3,11, 6},  },
            new List<int[]> { new int[]{ 3,10, 0}, new int[]{10, 8, 0}, new int[]{ 9, 4, 6},  },
            new List<int[]> { new int[]{ 3,10, 8}, new int[]{ 3, 8, 0}, new int[]{11, 4, 7}, new int[]{11, 9, 4},  },
            new List<int[]> { new int[]{ 8, 0, 5}, new int[]{ 5, 0, 7}, new int[]{ 0, 3, 7}, new int[]{ 4, 6, 9},  },
            new List<int[]> { new int[]{11, 9, 3}, new int[]{ 3, 9, 0}, new int[]{ 4, 5, 8},  },
            new List<int[]> { new int[]{ 5, 4,10}, new int[]{10, 4, 3}, new int[]{ 4, 0, 3},  },
            new List<int[]> { new int[]{ 5, 4,10}, new int[]{10, 4, 3}, new int[]{ 4, 0, 3}, new int[]{ 7,11, 6},  },
            new List<int[]> { new int[]{ 4, 0, 3}, new int[]{ 4, 3, 7},  },
            new List<int[]> { new int[]{11, 6, 3}, new int[]{ 6, 4, 3}, new int[]{ 3, 4, 0},  },
            new List<int[]> { new int[]{ 9, 5, 6}, new int[]{ 9, 3, 5}, new int[]{ 9, 0, 3}, new int[]{ 5, 3,10},  },
            new List<int[]> { new int[]{ 0, 3, 9}, new int[]{ 9, 3,11}, new int[]{10, 5, 7},  },
            new List<int[]> { new int[]{ 6, 9, 7}, new int[]{ 9, 0, 7}, new int[]{ 7, 0, 3},  },
            new List<int[]> { new int[]{ 0, 3, 9}, new int[]{ 9, 3,11},  },
            new List<int[]> { new int[]{ 2,11, 0}, new int[]{ 0,11, 8}, new int[]{11,10, 8},  },
            new List<int[]> { new int[]{ 0,10, 8}, new int[]{ 0, 6,10}, new int[]{ 0, 2, 6}, new int[]{10, 6, 7},  },
            new List<int[]> { new int[]{ 8, 7, 5}, new int[]{ 0, 7, 8}, new int[]{ 0,11, 7}, new int[]{ 0, 2,11},  },
            new List<int[]> { new int[]{ 8, 0, 5}, new int[]{ 0, 2, 5}, new int[]{ 5, 2, 6},  },
            new List<int[]> { new int[]{ 2,11, 0}, new int[]{ 0,11, 8}, new int[]{11,10, 8}, new int[]{ 9, 4, 6},  },
            new List<int[]> { new int[]{10, 8, 7}, new int[]{ 7, 8, 4}, new int[]{ 0, 2, 9},  },
            new List<int[]> { new int[]{ 0, 2, 9}, new int[]{ 8, 4, 5}, new int[]{11, 7, 6},  },
            new List<int[]> { new int[]{ 8, 4, 5}, new int[]{ 0, 2, 9},  },
            new List<int[]> { new int[]{ 2, 4, 0}, new int[]{ 2,10, 4}, new int[]{ 2,11,10}, new int[]{10, 5, 4},  },
            new List<int[]> { new int[]{ 2, 6, 0}, new int[]{ 0, 6, 4}, new int[]{ 7,10, 5},  },
            new List<int[]> { new int[]{ 2,11, 0}, new int[]{11, 7, 0}, new int[]{ 0, 7, 4},  },
            new List<int[]> { new int[]{ 4, 0, 6}, new int[]{ 6, 0, 2},  },
            new List<int[]> { new int[]{ 5, 6,10}, new int[]{10, 6,11}, new int[]{ 9, 0, 2},  },
            new List<int[]> { new int[]{10, 5, 7}, new int[]{ 2, 9, 0},  },
            new List<int[]> { new int[]{ 2, 9, 0}, new int[]{11, 7, 6},  },
            new List<int[]> { new int[]{ 2, 9, 0},  },
            new List<int[]> { new int[]{ 1, 8, 2}, new int[]{ 8, 9, 2},  },
            new List<int[]> { new int[]{ 1, 8, 2}, new int[]{ 8, 9, 2}, new int[]{11, 6, 7},  },
            new List<int[]> { new int[]{ 9, 2, 8}, new int[]{ 2, 1, 8}, new int[]{ 5,10, 7},  },
            new List<int[]> { new int[]{ 1, 8, 9}, new int[]{ 1, 9, 2}, new int[]{10, 6, 5}, new int[]{10,11, 6},  },
            new List<int[]> { new int[]{ 4, 6, 8}, new int[]{ 8, 6, 1}, new int[]{ 6, 2, 1},  },
            new List<int[]> { new int[]{11, 4, 7}, new int[]{11, 1, 4}, new int[]{11, 2, 1}, new int[]{ 4, 1, 8},  },
            new List<int[]> { new int[]{ 4, 6, 8}, new int[]{ 8, 6, 1}, new int[]{ 6, 2, 1}, new int[]{ 5,10, 7},  },
            new List<int[]> { new int[]{ 2, 1,11}, new int[]{11, 1,10}, new int[]{ 8, 4, 5},  },
            new List<int[]> { new int[]{ 9, 2, 4}, new int[]{ 4, 2, 5}, new int[]{ 2, 1, 5},  },
            new List<int[]> { new int[]{ 9, 2, 4}, new int[]{ 4, 2, 5}, new int[]{ 2, 1, 5}, new int[]{ 6, 7,11},  },
            new List<int[]> { new int[]{ 9, 7, 4}, new int[]{ 9, 1, 7}, new int[]{ 9, 2, 1}, new int[]{ 1,10, 7},  },
            new List<int[]> { new int[]{10,11, 1}, new int[]{ 1,11, 2}, new int[]{ 6, 4, 9},  },
            new List<int[]> { new int[]{ 6, 2, 1}, new int[]{ 6, 1, 5},  },
            new List<int[]> { new int[]{ 7,11, 5}, new int[]{11, 2, 5}, new int[]{ 5, 2, 1},  },
            new List<int[]> { new int[]{10, 7, 1}, new int[]{ 7, 6, 1}, new int[]{ 1, 6, 2},  },
            new List<int[]> { new int[]{10,11, 1}, new int[]{ 1,11, 2},  },
            new List<int[]> { new int[]{ 1, 8, 3}, new int[]{ 3, 8,11}, new int[]{ 8, 9,11},  },
            new List<int[]> { new int[]{ 6, 8, 9}, new int[]{ 6, 3, 8}, new int[]{ 6, 7, 3}, new int[]{ 3, 1, 8},  },
            new List<int[]> { new int[]{ 1, 8, 3}, new int[]{ 3, 8,11}, new int[]{ 8, 9,11}, new int[]{10, 7, 5},  },
            new List<int[]> { new int[]{ 6, 5, 9}, new int[]{ 9, 5, 8}, new int[]{10, 3, 1},  },
            new List<int[]> { new int[]{ 4, 1, 8}, new int[]{ 6, 1, 4}, new int[]{ 6, 3, 1}, new int[]{ 6,11, 3},  },
            new List<int[]> { new int[]{ 1, 8, 3}, new int[]{ 8, 4, 3}, new int[]{ 3, 4, 7},  },
            new List<int[]> { new int[]{ 6,11, 7}, new int[]{ 4, 5, 8}, new int[]{ 3, 1,10},  },
            new List<int[]> { new int[]{ 5, 8, 4}, new int[]{10, 3, 1},  },
            new List<int[]> { new int[]{ 3, 9,11}, new int[]{ 3, 5, 9}, new int[]{ 3, 1, 5}, new int[]{ 9, 5, 4},  },
            new List<int[]> { new int[]{ 1, 5, 3}, new int[]{ 3, 5, 7}, new int[]{ 4, 9, 6},  },
            new List<int[]> { new int[]{ 9,11, 4}, new int[]{ 4,11, 7}, new int[]{ 3, 1,10},  },
            new List<int[]> { new int[]{ 3, 1,10}, new int[]{ 6, 4, 9},  },
            new List<int[]> { new int[]{11, 3, 6}, new int[]{ 3, 1, 6}, new int[]{ 6, 1, 5},  },
            new List<int[]> { new int[]{ 7, 3, 5}, new int[]{ 5, 3, 1},  },
            new List<int[]> { new int[]{10, 3, 1}, new int[]{ 7, 6,11},  },
            new List<int[]> { new int[]{ 3, 1,10},  },
            new List<int[]> { new int[]{ 3,10, 2}, new int[]{ 2,10, 9}, new int[]{10, 8, 9},  },
            new List<int[]> { new int[]{ 3,10, 2}, new int[]{ 2,10, 9}, new int[]{10, 8, 9}, new int[]{11, 6, 7},  },
            new List<int[]> { new int[]{ 2, 8, 9}, new int[]{ 2, 7, 8}, new int[]{ 2, 3, 7}, new int[]{ 8, 7, 5},  },
            new List<int[]> { new int[]{ 8, 9, 5}, new int[]{ 5, 9, 6}, new int[]{ 2, 3,11},  },
            new List<int[]> { new int[]{ 3, 6, 2}, new int[]{ 3, 8, 6}, new int[]{ 3,10, 8}, new int[]{ 8, 4, 6},  },
            new List<int[]> { new int[]{ 4, 7, 8}, new int[]{ 8, 7,10}, new int[]{11, 2, 3},  },
            new List<int[]> { new int[]{ 3, 7, 2}, new int[]{ 2, 7, 6}, new int[]{ 5, 8, 4},  },
            new List<int[]> { new int[]{ 8, 4, 5}, new int[]{ 3,11, 2},  },
            new List<int[]> { new int[]{ 9, 5, 4}, new int[]{ 2, 5, 9}, new int[]{ 2,10, 5}, new int[]{ 2, 3,10},  },
            new List<int[]> { new int[]{ 2, 3,11}, new int[]{ 9, 6, 4}, new int[]{10, 5, 7},  },
            new List<int[]> { new int[]{ 9, 2, 4}, new int[]{ 2, 3, 4}, new int[]{ 4, 3, 7},  },
            new List<int[]> { new int[]{ 9, 6, 4}, new int[]{ 2, 3,11},  },
            new List<int[]> { new int[]{ 3,10, 2}, new int[]{10, 5, 2}, new int[]{ 2, 5, 6},  },
            new List<int[]> { new int[]{ 7,10, 5}, new int[]{11, 2, 3},  },
            new List<int[]> { new int[]{ 3, 7, 2}, new int[]{ 2, 7, 6},  },
            new List<int[]> { new int[]{11, 2, 3},  },
            new List<int[]> { new int[]{ 8, 9,11}, new int[]{ 8,11,10},  },
            new List<int[]> { new int[]{ 6, 7, 9}, new int[]{ 7,10, 9}, new int[]{ 9,10, 8},  },
            new List<int[]> { new int[]{ 7, 5,11}, new int[]{ 5, 8,11}, new int[]{11, 8, 9},  },
            new List<int[]> { new int[]{ 6, 5, 9}, new int[]{ 9, 5, 8},  },
            new List<int[]> { new int[]{ 4, 6, 8}, new int[]{ 6,11, 8}, new int[]{ 8,11,10},  },
            new List<int[]> { new int[]{ 4, 7, 8}, new int[]{ 8, 7,10},  },
            new List<int[]> { new int[]{ 7, 6,11}, new int[]{ 5, 8, 4},  },
            new List<int[]> { new int[]{ 8, 4, 5},  },
            new List<int[]> { new int[]{ 5, 4,10}, new int[]{ 4, 9,10}, new int[]{10, 9,11},  },
            new List<int[]> { new int[]{ 6, 4, 9}, new int[]{ 7,10, 5},  },
            new List<int[]> { new int[]{ 9,11, 4}, new int[]{ 4,11, 7},  },
            new List<int[]> { new int[]{ 9, 6, 4},  },
            new List<int[]> { new int[]{ 5, 6,10}, new int[]{10, 6,11},  },
            new List<int[]> { new int[]{10, 5, 7},  },
            new List<int[]> { new int[]{ 7, 6,11},  },
            new List<int[]> {  }
        };
    
    
    public static IsosurfaceData IsosurfaceGen
        (double[,,] density, double[] origin, double[,] axes, double thresh, bool inverse=false)
    {
        int[] nGrid = { density.GetLength(0), density.GetLength(1), density.GetLength(2) };
        int[,,] isInRegion = new int[nGrid[0], nGrid[1], nGrid[2]];
        int[,,,] vertIndex = new int[3, nGrid[0], nGrid[1], nGrid[2]];

        var vertCoordinates = new List<double[]>();
        var polygons = new List<int[]>();

        for (int ix = 0; ix < nGrid[0]; ++ix)
        for (int iy = 0; iy < nGrid[1]; ++iy)
        for (int iz = 0; iz < nGrid[2]; ++iz){
            if (density[ix, iy, iz] > thresh && !inverse) { isInRegion[ix, iy, iz] = 1; }
            if (density[ix, iy, iz] < thresh &&  inverse) { isInRegion[ix, iy, iz] = 1; }
        }

        int nextVertIndex = 0;
        for (int iaxis = 0; iaxis < 3; ++iaxis){
            int isXedge = 0, isYedge = 0, isZedge =0 ;
            switch(iaxis){
                case 0: isXedge = 1; break;
                case 1: isYedge = 1; break;
                case 2: isZedge = 1; break;
            }
            
            for (int ix = 0; ix < nGrid[0] - isXedge; ++ix)
            for (int iy = 0; iy < nGrid[1] - isYedge; ++iy)
            for (int iz = 0; iz < nGrid[2] - isZedge; ++iz){
                vertIndex[iaxis, ix, iy, iz] = -1;
                if(isInRegion[ix, iy, iz] != isInRegion[ix + isXedge, iy + isYedge, iz + isZedge]){
                    vertIndex[iaxis, ix, iy, iz] = nextVertIndex;
                    nextVertIndex++;

                    double f0 = density[ix, iy, iz];
                    double f1 = density[ix + isXedge, iy + isYedge, iz + isZedge];
                    double[] coord = {ix, iy, iz};
                    coord[iaxis] += (f0 - thresh) / (f0 - f1);
                    for(int jaxis = 0; jaxis < 3; ++jaxis){
                        coord[jaxis] = coord[jaxis] * axes[jaxis, jaxis] + origin[jaxis];
                    }
                    vertCoordinates.Add(coord);
                }
            }
        }

        for (int ix = 0; ix < nGrid[0]-1; ++ix)
        for (int iy = 0; iy < nGrid[1]-1; ++iy)
        for (int iz = 0; iz < nGrid[2]-1; ++iz){
            int key
                = isInRegion[ix  , iy  , iz  ] * 128
                + isInRegion[ix+1, iy  , iz  ] *  64
                + isInRegion[ix  , iy+1, iz  ] *  32
                + isInRegion[ix+1, iy+1, iz  ] *  16
                + isInRegion[ix  , iy  , iz+1] *   8
                + isInRegion[ix+1, iy  , iz+1] *   4
                + isInRegion[ix  , iy+1, iz+1] *   2
                + isInRegion[ix+1, iy+1, iz+1];

            foreach(int[] plane in PlanePattern[key]){
                var polygon = new int[3];
                for(int i=0; i<3; ++i){
                    int[] e = EdgeDirAndStart[plane[i]];
                    polygon[i] = vertIndex[e[0], ix + e[1], iy + e[2], iz + e[3]];
                }
                polygons.Add(polygon);
            }

        }
        
        return new IsosurfaceData{ VertCoordinates = vertCoordinates, Polygons = polygons };
        
    }

    public static void Main(){
        var cubeData = CubeData.Load("h2o.isodebug.cube");
        List<double[,,]> data = cubeData.Data;
        double[] origin = cubeData.Origin;
        double[,] axes = cubeData.Axes;

        
        int iMO = 0;
        //var moVal = new double[data.GetLength(0),data.GetLength(1),data.GetLength(2)];
        //for(int ix=0; ix < data.GetLength(0); ++ix)
        //for(int iy=0; iy < data.GetLength(1); ++iy)
        //for(int iz=0; iz < data.GetLength(2); ++iz)
        //    moVal[ix,iy,iz] = data[ix,iy,iz,iMO];
        
        var isosurfacePosi = IsosurfaceGen(data[iMO], origin, axes,  0.020, false);
        var isosurfaceNega = IsosurfaceGen(data[iMO], origin, axes, -0.020, true );
    }
    
}
