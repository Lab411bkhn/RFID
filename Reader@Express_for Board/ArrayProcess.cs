using System;

using System.Collections.Generic;
using System.Text;

namespace Reader_Express
{
    class ArrayProcess
    {
        // hàm xóa tất cả giá trị trong mảng
        public void Clear(int[] A)
        {
            if (A[0] != 0)
            {
                int length = A.Length;
                for (int i = 0; i <= length; i++)
                    A[i] = 0;
            }

        }
        //hàm thêm giá trị vào vị trí trống trong mảng
        public void AddIn(int[] A , int x )
        {
            int length = A.Length;
            int i = 0;
            while (A[i] != 0) i++;
            A[i] = x; 
        }
        public void AddInFrist(int[] A, int x)
        {
            if (A[0] != null)
            {
                int length = A.Length;
                int i = 0;
                while (A[i] != 0) i++;
                for (int j = i - 1; j >= 0; j--)
                {
                    A[j + 1] = A[j];
                }
                if (A[0] == 0) A[0] = x;
            }
            else A[0] = x;
        }
           
    }
}
