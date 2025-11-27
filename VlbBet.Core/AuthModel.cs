using System;
using System.Collections.Generic;
using System.Text;

namespace VlbBet.Core
{
    // Lo que tu API espera en el body del login
    public class AuthRequest
    {
        public string level { get; set; }      // "4"
        public string username { get; set; }   // celular / usuario
        public string password { get; set; }   // contraseña
    }

    // Lo que tu API devuelve (ajusta los nombres si son distintos)
    public class AuthResponse
    {
        public string sectionId { get; set; } 
        public string user { get; set; }  
        public string name { get; set; }
        public string level { get; set; }        
        public string point { get; set; }  
    }

    public class Response
    {
        public string Message { get; set; }
    }
}
