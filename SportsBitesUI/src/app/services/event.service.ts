import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventDto } from '../models/event';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  // Ensure this port matches your .NET project (check launchSettings.json)
  private apiUrl = 'http://localhost:5283/api/events';

  constructor(private http: HttpClient) {
  }

  getEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(this.apiUrl);
  }

  getEvent(id: number, voterName: string): Observable<EventDto> {
    return this.http.get<EventDto>(`${this.apiUrl}/${id}?voterName=${voterName}`);
  }

  upvoteFood(foodId: number, voterName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/food/${foodId}/upvote?voterName=${voterName}`, {});
  }

  addFoodSuggestion(eventId: number, suggestion: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${eventId}/food`, suggestion);
  }

  createEvent(newEvent: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, newEvent);
  }
}
