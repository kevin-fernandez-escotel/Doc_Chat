namespace Doc_Chat.Entidades
{
    public class Mensaje
    {
        public string EmisorId { get; set; }
        public string EmisorNombre { get; set; }
        public DateTime FechaMensaje { get; set; }

        //1 => Texto
        //2 => Archivo
        public int TipoMensaje { get; set; }
        public string Texto { get; set; }
        public string UrlArchivo { get; set; }

        //1 => Cabina
        //2 => Proveedores
        public int TipoUsuario { get; set; }
    }
}
