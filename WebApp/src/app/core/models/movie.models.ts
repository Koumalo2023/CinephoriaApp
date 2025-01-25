import { MovieGenre } from "./enum.model";
import { MovieRatingDto } from "./movie-rating.models";
import { ShowtimeDto } from "./showtime.models";

export interface MovieDto {
    movieId: number;
    title: string;
    description: string;
    genre: MovieGenre;
    duration: string;
    director: string;
    releaseDate: Date;
    minimumAge: number;
    isFavorite: boolean;
    averageRating: number;
    posterUrls: string[];
    showtimes: ShowtimeDto[];
    movieRatings: MovieRatingDto[];
  }

export interface MovieDetailsDto {
    movieId: number;
    title: string;
    description: string;
    genre: MovieGenre;
    duration: string;
    director: string;
    releaseDate: Date;
    minimumAge: number;
    averageRating: number;
    posterUrls: string[];
    showtimes: ShowtimeDto[];
    ratings: MovieRatingDto[];
  }

export interface CreateMovieDto {
    title: string;
    description: string;
    genre: MovieGenre;
    duration: string;
    director: string;
    releaseDate: Date;
    minimumAge: number;
    posterUrls: string[];
  }

  export interface UpdateMovieDto {
    movieId: number;
    title: string;
    description: string;
    genre: MovieGenre;
    duration: string;
    director: string;
    releaseDate: Date;
    minimumAge: number;
    isFavorite: boolean;
    posterUrls: string[];
  }

  export interface MovieReviewDto {
    movieId: number;
    userId: string;
    rating: number;
    description?: string;
  }

  export interface FilterMoviesRequestDto {
    cinemaId?: number;
    genre?: MovieGenre;
    date?: Date;
  }
  
  