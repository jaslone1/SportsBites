export interface FoodSuggestionDto {
  foodSuggestionId: number;
  foodName: string;
  suggestedByName: string;
  upvoteCount: number;
  hasUserUpvoted: boolean;
}

export interface EventDto {
  eventId: number;
  eventName: string;
  eventDate: string;
  hostName: string;
  gameDetails: string;
  foodSuggestions: FoodSuggestionDto[];
}
