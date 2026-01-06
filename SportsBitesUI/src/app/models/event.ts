export interface FoodSuggestionDto {
  foodSuggestionId: number;
  foodName: string;
  suggestedByName: string;
  suggestedByUserId: string;
  upvoteCount: number;
  hasUserUpvoted: boolean;
  claimedByName?: string;
  claimedByUserId?: string;
}

export interface EventDto {
  eventId: number;
  eventName: string;
  eventDate: string;
  hostName: string;
  hostUserId: string;
  gameDetails: string;
  foodSuggestions: FoodSuggestionDto[];
}
