

using System.Security;
using System.Text.RegularExpressions;

namespace analizador
{
    internal class Program
    {

        #region Variables Globales

        #region Diccionario Tokens
        public static Dictionary<int, string> diccionarioTokens = new Dictionary<int, string>
    {
        { (int)TokenType.ERROR,"ERROR"},
        { (int)TokenType.IF, "IF" },
        { (int)TokenType.ELSE, "ELSE" },
        { (int)TokenType.WHILE, "WHILE" },
        { (int)TokenType.FOR, "FOR" },
        { (int)TokenType.OPERADOR_ARITMETICO, "OPERADOR ARITMÉTICO" },
        { (int)TokenType.OPERADOR_LOGICO, "OPERADOR LÓGICO" },
        { (int)TokenType.OPERADOR_ASIGNACION,"OPERADOR DE ASIGNACIÓN" },
        { (int)TokenType.OPERADOR_RELACIONAL,"OPERADOR RELACIONAL" },
        { (int)TokenType.PARENTESIS_IZQUIERDO, "PARENTESIS IZQUIERDO" },
        { (int)TokenType.PARENTESIS_DERECHO, "PARENTESIS DERECHO" },
        { (int)TokenType.LLAVE_IZQUIERDA, "LLAVE IZQUIERDA" },
        { (int)TokenType.LLAVE_DERECHA, "LLAVE DERECHA" },
        { (int)TokenType.CORCHETE_DERECHO, "CORCHETE IZQUIERDO" },
        { (int)TokenType.CORCHETE_IZQUIERDO, "CORCHETE DERECHO" },
        { (int)TokenType.PUNTO_Y_COMA, "PUNTO Y COMA" },
        { (int)TokenType.COMA, "COMA" },
        { (int)TokenType.IDENTIFICADOR, "IDENTIFICADOR" },
        { (int)TokenType.ENTERO, "ENTERO" },
        { (int)TokenType.DECIMAL, "DECIMAL" },
        { (int)TokenType.FIN_DE_ARCHIVO, "EOF" }
     };



        #endregion

        #region Struct Token
        struct Token
        {
            public string Nombre;
            public int Valor;
            public string Lexema;
        }

        #endregion

        private static string input = "";
        private static List<Token> lexico = new List<Token>();
        private static string nombreArchivo = "../../../cadena.txt";

        #endregion

        private static void Main(string[] args)
        {
            AnalizadorLexico(input);
        }

        #region Analizador Léxico
        private static void AnalizadorLexico(string input)
        {

            input = IngresarCadena();
            AnalizarCadena(input);

        }

        #endregion

        #region Función ingresar cadena
        private static string IngresarCadena()
        {
            string value;
            #region Consola
            //Console.Write("Ingresa la cadena: ");
            //#pragma warning disable CS8600
            //value = Console.ReadLine(); ;
            //if (value == null)
            //{
            //    value = "";
            //}
            //#pragma warning restore CS8600
            #endregion
            #region Archivo 
            value = File.ReadAllText(nombreArchivo);
            Console.WriteLine("Cadena:\n");
            Console.WriteLine(value + '\n');
            #endregion
            return value + "\0";
        }
        #endregion

        #region Función saltar espacios
        private static int SaltarEspacios(string input, int i)
        {
            while (char.IsSeparator(input[i]) || Regex.IsMatch(input[i].ToString(),@"[\n\t\r\b\v]"))
            {
                i++;
            }
            return i;
        }
        #endregion

        #region Función analizar cadena
        private static void AnalizarCadena(string input)
        {

            if (input != null)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    // Variables 

                    i = SaltarEspacios(input, i);
                    Token token = new Token();
                    token.Lexema = "";

                    #region Condiciones para obtener lexemas

                    if (char.IsDigit(input[i]) || input[i].Equals('.'))
                    {
                        #region INT Y FLOAT NÚMEROS

                        while (char.IsDigit(input[i]))
                        {

                            token.Lexema += input[i];
                            i++;
                        }
                        // Número entero
                        token.Valor = (int)TokenType.ENTERO;
                        if (input[i].Equals('.'))
                        {
                            token.Lexema += input[i];
                            i++;
                            if (input[i].Equals('\0'))
                            {
                                token.Valor = 0;

                            }
                            else
                            {
                                while (char.IsDigit(input[i]))
                                {
                                    token.Lexema += input[i];
                                    i++;
                                }
                                // Número decimal
                                token.Valor = (int)TokenType.DECIMAL;


                            }


                        }
                        //Regresar a posición
                        i--;

                        #endregion
                    }
                    else if (char.IsLetter(input[i]) || input[i].Equals('_'))
                    {
                        #region ID y PALABRAS RESERVADAS
                        token.Lexema += input[i];
                        i++;

                        while (char.IsLetterOrDigit(input[i]) || Regex.IsMatch(input[i].ToString(), @"[_]"))
                        {
                            token.Lexema += input[i];
                            i++;
                        }
                        token.Valor = diccionarioTokens.FirstOrDefault(x => x.Value == token.Lexema.ToUpper()).Key;
                        //ID
                        if (token.Valor == 0)
                        {
                            token.Valor = (int)TokenType.IDENTIFICADOR;
                        }
                        //Regresar al elemento anterior
                        i--;
                        #endregion

                    }
                    else if (Regex.IsMatch(input[i].ToString(), @"[%/+*-]"))
                    {
                        #region OPERADORES ARITMÉTICOS
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.OPERADOR_ARITMETICO;
                        #endregion
                    }
                    else if (Regex.IsMatch(input[i].ToString(), @"[!=&<>\|]"))
                    {
                        #region OPERADORES RELACIONALES Y LÓGICOS
                        string cadena = "" + input[i] + input[i + 1];

                        token.Lexema += input[i];

                        if (Regex.IsMatch(cadena, @"(!=|==|>=|<=)"))
                        {
                            #region OPERADOR RELACIONAL DE DOS CARACTERES
                            i++;
                            token.Lexema += input[i];
                            token.Valor = (int)TokenType.OPERADOR_RELACIONAL;
                            #endregion
                        }
                        else if (Regex.IsMatch(cadena, @"[<>]"))
                        {
                            #region OPERADOR RELACIONAL DE UN CARACTÉR
                            token.Valor = (int)TokenType.OPERADOR_RELACIONAL;
                            #endregion
                        }
                        else if (Regex.IsMatch(cadena, @"(&&|\|\|)"))
                        {
                            #region OPERADOR LÓGICO DE DOS CARACTERES
                            i++;
                            token.Lexema += input[i];
                            token.Valor = (int)TokenType.OPERADOR_LOGICO;
                            #endregion
                        }
                        else if (input[i].Equals('!'))
                        {
                            #region OPERADOR LÓGICO DE UN CARACTER
                            token.Valor = (int)TokenType.OPERADOR_LOGICO;
                            #endregion
                        }
                        else if (input[i].Equals('='))
                        {
                            #region OPERADOR DE ASIGNACIÓN
                            token.Valor = (int)TokenType.OPERADOR_ASIGNACION;
                            #endregion
                        }
                        #endregion
                    }

                    #region LEXÉMAS DE UN CARACTER
                    else if (input[i].Equals('('))
                    {
                        #region (
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.PARENTESIS_IZQUIERDO;
                        #endregion
                    }
                    else if (input[i].Equals(')'))
                    {
                        #region )
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.PARENTESIS_DERECHO;
                        #endregion
                    }
                    else if (input[i].Equals('{'))
                    {
                        #region {
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.LLAVE_IZQUIERDA;
                        #endregion
                    }
                    else if (input[i].Equals('}'))
                    {
                        #region }
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.LLAVE_DERECHA;
                        #endregion
                    }
                    else if (input[i].Equals('['))
                    {
                        #region [
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.CORCHETE_IZQUIERDO;
                        #endregion
                    }
                    else if (input[i].Equals(']'))
                    {
                        #region ]
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.CORCHETE_DERECHO;
                        #endregion
                    }
                    else if (input[i].Equals(';'))
                    {
                        #region ;
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.PUNTO_Y_COMA;
                        #endregion
                    }
                    else if (input[i].Equals(','))
                    {
                        #region ,
                        token.Lexema += input[i];
                        token.Valor = (int)TokenType.COMA;
                        #endregion
                    }
                    #endregion

                    else if (input[i].Equals('\0'))
                    {
                        #region FIN
                        token.Lexema += "\\0";
                        token.Valor = (int)TokenType.FIN_DE_ARCHIVO;
                        #endregion
                    }
                    else
                    {
                        #region ERROR
                        token.Lexema += input[i].ToString();
                        token.Valor = 0;
                        #endregion
                    }
                    #endregion

                    token.Nombre = diccionarioTokens[token.Valor];
                    lexico.Add(token);
                    
                  


                }

                #region Imprimir tokens
                Console.WriteLine("Lexema".PadRight(20) + "Nombre".PadRight(30) + "Valor".PadRight(10));
                Console.WriteLine(new string('-', 60)); // Línea separadora
                foreach (Token token in lexico)
                {
                    Console.WriteLine(token.Lexema.PadRight(20) + token.Nombre.PadRight(30) +
                        token.Valor.ToString().PadRight(10));
                }
                #endregion

            }
        }
        #endregion

    }
}
