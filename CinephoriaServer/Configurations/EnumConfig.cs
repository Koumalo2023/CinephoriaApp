﻿namespace CinephoriaServer.Configurations
{
    public static class EnumConfig
    {

        public enum UserRole
        {
            User,
            Employee,
            Admin
        }

        public enum ReservationStatus
        {
            Pending,
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


        public enum MovieGenre
        {

            Action,
            Aventure,
            Comedie,
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
            Mystere,

        }
    }
}
