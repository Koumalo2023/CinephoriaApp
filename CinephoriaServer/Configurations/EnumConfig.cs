namespace CinephoriaServer.Configurations
{
    public static class EnumConfig
    {

        public enum UserRole
        {
            ADMIN,
            EMPLOYEE,
            USER
        }

        public enum ReservationStatus
        {
            CONFIRMED,
            CANCELLED
        }

        public enum ProjectionQuality
        {
            /// <summary>
            /// Projection en 4DX.
            /// </summary>
            FourDX,

            /// <summary>
            /// Projection en 3D.
            /// </summary>
            ThreeD,

            /// <summary>
            /// Projection en IMAX.
            /// </summary>
            IMAX,

            /// <summary>
            /// Projection en 4K.
            /// </summary>
            FourK,

            /// <summary>
            /// Projection classique (2D).
            /// </summary>
            Standard2D,

            /// <summary>
            /// Projection Dolby Cinema.
            /// </summary>
            DolbyCinema
        }


        public enum IncidentStatus
        {
            /// <summary>
            /// Incident en attente de traitement.
            /// </summary>
            Pending,

            /// <summary>
            /// Incident en cours de traitement.
            /// </summary>
            InProgress,

            /// <summary>
            /// Incident résolu.
            /// </summary>
            Resolved
        }
    }
}
