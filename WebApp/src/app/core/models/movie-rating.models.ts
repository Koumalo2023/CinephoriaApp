
export interface MovieRatingDto {
  movieRatingId: number;
  movieId: number;
  appUserId: string;
  rating: number;
  comment?: string;
  isValidated: boolean;
}

export interface CreateMovieRatingDto {
  movieId: number;
  appUserId: string;
  rating: number;
  comment?: string;
}


export interface MovieReviewDto {
  movieId: number;
  userId: string;
  rating: number;
  description?: string;
}

export interface UpdateMovieRatingDto {
  movieRatingId: number;
  rating: number;
  comment?: string;
}