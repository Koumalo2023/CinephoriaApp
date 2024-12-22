namespace CinephoriaServer.Configurations
{
    public static class EnumConfig
    {

        public enum UserRole
        {
            Admin,
            Eemployee,
            User
        }

        public enum ReservationStatus
        {
            Confirmed,
            Cancelled
        }

        public enum ProjectionQuality
        {
            FourDX,
            ThreeD,
            IMAX,
            FourK,
            Standard2D,
            DolbyCinema
        }


        public enum IncidentStatus
        {
            Pending,
            InProgress,
            Resolved
        }


        public enum MovieGenreStatus
        {

            Action,
            Aventure,
            Comédie,
            Animation,
            Crime,
            Documentaire,
            Fantastique,
            Guerre,
            Horreur,
            Western,
            Romance, 
            Familiale,
            Thriller,
            Mystère,
            Comedie,

        }
    }
}
