import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';
import { ActivatedRoute } from '@angular/router';
import { filter, map, distinctUntilChanged } from 'rxjs/operators';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-details-view',
  templateUrl: './event-details.component.html',
  standalone: false
})
export class EventDetailsComponent implements OnInit {
  event: any = null;
  newFoodName = '';
  currentUserName: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {
    this.currentUserName = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    this.currentUserName = this.authService.getCurrentUser();
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id && id !== '0') {
        this.loadEvent(Number(id));
      }
    });
  }

  loadEvent(id: number) {
    this.eventService.getEvent(id).subscribe({
      next: (data) => {
        this.event = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("API Error:", err);
      }
    });
  }

  onVote(foodId: number) {
    this.eventService.upvoteFood(foodId).subscribe(() => {
      if (this.event) {
        this.loadEvent(this.event.eventId);
      }
    });
  }

  suggestFood() {
    if (!this.event || !this.newFoodName.trim()) return;
    const suggestion = {
      foodName: this.newFoodName
    };

    this.eventService.addFoodSuggestion(this.event.eventId, suggestion).subscribe({
      next: () => {
        this.newFoodName = '';
        this.loadEvent(this.event.eventId);
      }
    });
  }

  onClaim(foodId: number) {
    this.eventService.claimFood(foodId).subscribe({
      next: () => {
        this.loadEvent(this.event.eventId);
      }
    });
  }

  onUnclaim(foodId: number) {
    this.eventService.unclaimFood(foodId).subscribe({
      next: () =>
        this.loadEvent(this.event.eventId)
    });
  }
}
