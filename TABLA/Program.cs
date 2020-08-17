using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using ComandosConsola;
using Numerico;

namespace TABLA
{
    class Program
    {
        //COLORES
        public static ConsoleColor GS = ConsoleColor.Gray,
                                   BL = ConsoleColor.White,

                                   RJ = ConsoleColor.Red,
                                   VR = ConsoleColor.Green,
                                   
                                   NG = ConsoleColor.Black,

                                   AZ = ConsoleColor.Blue,
                                   CY = ConsoleColor.Cyan;

        public static Consola consola = new Consola("Tabla", BL);
        public static Numero n = new Numero();

        public static string[] tbV =
            {
                "-----",
                "--1--",
                "-234-",
                "-567-",
                "--8--",
                "-----",
            };
        public static ConsoleColor[,] tbC =
        {
                { NG,NG,NG,NG,NG},
                { NG,NG,BL,NG,NG},
                { NG,BL,BL,BL,NG},
                { NG,BL,BL,BL,NG},
                { NG,NG,BL,NG,NG},
                { NG,NG,NG,NG,NG},
            };

        public static int columnas = 5, filas = 6;
        public static dynamic[,,] tabla = new dynamic[filas, columnas, 2];

        public static List<int> candidatoX = new List<int>();
        public static List<int> candidatoY = new List<int>();

        public static string numSelec = "", numeros = "12345678";

        public static int posB,//posicion en barra
                          xs = 0, ys = 0,//posicion selecionado
                          posAntes = 0, posDesp = 0,
                          candidatos = 0;

        public static bool actAntes = false, actDesp = false;

        public static int[] candidatoAntes = new int[2],
                            candidatoDespues = new int[2];


        static void Main()
        {
            Inicializar();

            Mostrar();

            for (int fil = 0; fil < filas; fil++)
            {
                for (int col = 0; col < columnas; col++)
                {
                    xs = col; ys = fil;

                    numSelec = tabla[ys, xs, 0];
                    n.TXT = numSelec;

                    if (n.NumeroV())//buscar cual es numero
                    {
                        ColorM(xs, ys, AZ);//y seleccionarlo
                        Mostrar();

                        Candidatos(xs, ys, numSelec);//busacar posibles candidatos
                        Mostrar();

                        if (Localizar())//localizar selececionado
                        {
                            posAntes = posB - 1;
                            posDesp = posB + 1;

                            Antes();

                            Despues();
                            
                        }



                        consola.delay(1000);
                        Deselect(col, fil);
                    }
                }
            }


            //consola.escribe($"posicion ({caja[2,2,0]}, {caja[2,2,1]}), valor: {caja[2,2,2]}, color: {caja[2,2,3]}");
            consola.esperar();
        }

        public static void Despues()
        {
            for (int c = 0; c < candidatos; c++)
            {
                if (numeros[posDesp].ToString() != tabla[candidatoY[c], candidatoX[c], 0])
                {
                    ColorM(candidatoX[c], candidatoY[c], RJ);
                }
                else
                {
                    ColorM(candidatoX[c], candidatoY[c], VR);
                    candidatoDespues[0] = candidatoX[c];
                    candidatoDespues[1] = candidatoY[c];
                }
            }
        }
        public static void Antes()
        {
            for (int c = 0; c < candidatos; c++)
            {
                if (numeros[posAntes].ToString() != tabla[candidatoY[c], candidatoX[c], 0])
                {
                    ColorM(candidatoX[c], candidatoY[c], RJ);
                }
                else
                {
                    ColorM(candidatoX[c], candidatoY[c], VR);
                    candidatoAntes[0] = candidatoX[c];
                    candidatoAntes[1] = candidatoY[c];
                }
            }
        }
        public static bool Localizar()
        {
            bool encontrado = false;
            for (int i = 0; (i < numeros.Length) && (encontrado == false); i++)
            {
                if (tabla[ys, xs, 0] == numeros[i].ToString())
                {
                    posB = i;
                    encontrado = true;
                }
            }

            return encontrado;
        }

        public static void Deselect(int x, int y)
        {
            for (int fil = y - 1; fil < (y + 2); fil++)
            {
                for (int col = x - 1; col < (x + 2); col++)
                {
                    n.TXT = tabla[fil, col, 0];

                    if (n.NumeroV())
                    {
                        ColorM(col, fil, BL);

                        candidatoX.Remove(col);
                        candidatoY.Remove(fil);
                    }
                }
            }
            Mostrar();
        }
        public static void Candidatos(int x, int y, string numSelec)
        {
            string numAlrededor = "";

            for (int fil = y - 1; fil < (y + 2); fil++)
            {
                for (int col = x - 1; col < (x + 2); col++)
                {
                    numAlrededor = tabla[fil, col, 0];
                    n.TXT = numAlrededor;

                    if (n.NumeroV() && (numAlrededor != numSelec))
                    {
                        ColorM(col, fil, CY);

                        candidatoX.Add(col);
                        candidatoY.Add(fil);

                        candidatos++;
                    }
                }
            }
        }

        public static void ColorH(int x, int y, ConsoleColor color)
        {
            tabla[y - 1, x - 1, 1] = color;
        }
        public static void ColorM(int x, int y, ConsoleColor color)
        {
            tabla[y, x, 1] = color;
        }

        public static void ValorH(int x, int y, string valor)
        {
            tabla[y - 1, x - 1, 0] = valor;
        }
        public static void ValorM(int x, int y, string valor)
        {
            tabla[y, x, 0] = valor;
        }

        public static void Mostrar()
        {
            consola.limpiar();

            for (int fil = 0; fil < filas; fil++)
            {
                for (int col = 0; col < columnas; col++)
                {
                    Colocar(col, fil, tabla[fil, col, 0], tabla[fil, col, 1]);
                }
            }
            consola.delay(1000);
        }
        public static void Colocar(int x, int y, string valor, ConsoleColor color)
        {
            Consola c = new Consola("Tabla");
            c.colorGlobal = color;

            c.posConsola(5 * x, 0 + (3 * y));
            c.escribe("┌───┐");

            c.posConsola(5 * x, 1 + (3 * y));
            c.escribe($"│ {valor} │");

            c.posConsola(5 * x, 2 + (3 * y));
            c.escribe("└───┘");
        }

        public static void Inicializar()
        {
            for (int fil = 0; fil < filas; fil++)
            {
                for (int col = 0; col < columnas; col++)
                {
                    tabla[fil, col, 0] = tbV[fil][col].ToString();
                    tabla[fil, col, 1] = tbC[fil, col];
                }
            }
        }
    }
}
