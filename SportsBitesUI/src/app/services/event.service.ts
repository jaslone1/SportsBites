import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventDto } from '../models/event';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  private apiUrl = `${environment.apiUrl}/events`;
  private foodUrl = `${environment.apiUrl}/food`;

  // --- EVENT METHODS ---
  getEvents(): Observable<any[]> {
    return this.http.get<any[]>(this.eventUrl);
  }

  getEvent(id: number): Observable<any> {
    return this.http.get<any>(`${this.eventUrl}/${id}`);
  }

  createEvent(newEvent: any): Observable<any> {
    return this.http.post<any>(this.eventUrl, newEvent);
  }

  updateEvent(id: number, data: any): Observable<any> {
    return this.http.put(`${this.eventUrl}/${id}`, data);
  }

  finalizeEvent(id: number): Observable<any> {
    return this.http.patch(`${this.eventUrl}/${id}/finalize`, {});
  }

  deleteEvent(id: number): Observable<any> {
    return this.http.delete(`${this.eventUrl}/${id}`);
  }

  // FOOD METHODS
  addFoodSuggestion(eventId: number, suggestion: any): Observable<any> {
    return this.http.post(`${this.foodUrl}/${eventId}`, suggestion);
  }

  upvoteFood(foodId: number): Observable<any> {
    return this.http.post(`${this.foodUrl}/${foodId}/upvote`, {});
  }

  claimFood(foodId: number): Observable<any> {
    return this.http.put(`${this.foodUrl}/${foodId}/claim`, {});
  }

  unclaimFood(foodId: number): Observable<any> {
    return this.http.put(`${this.foodUrl}/${foodId}/unclaim`, {});
  }
}
