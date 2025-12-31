import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventDto } from '../models/event';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  private apiUrl = `${environment.apiUrl}/event`;

  constructor(private http: HttpClient) {}

  getEvents(): Observable<EventDto[]> {
    return this.http.get<EventDto[]>(this.apiUrl);
  }

  getEvent(id: number): Observable<EventDto> {
    return this.http.get<EventDto>(`${this.apiUrl}/${id}`);
  }

  upvoteFood(foodId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/food/${foodId}/upvote`, {});
  }

  addFoodSuggestion(eventId: number, suggestion: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/${eventId}/food`, suggestion);
  }

  createEvent(newEvent: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, newEvent);
  }

  claimFood(foodId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/food/${foodId}/claim`, {});
  }

  unclaimFood(foodId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/food/${foodId}/unclaim`, {});
  }
}
