import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';
import { ActivatedRoute, Router } from '@angular/router';
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
  currentUserId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {
    this.currentUserName = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    this.currentUserName = this.authService.getCurrentUser();
    this.currentUserId = this.authService.getUserId();

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

  onFinalize() {
    this.eventService.finalizeEvent(this.event.eventId).subscribe({
      next: () => {
        this.loadEvent(this.event.eventId); // Refresh UI
        console.log("Menu locked!");
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

  onDelete() {
    if (!this.event) return;
    if (confirm("Are you sure you want to cancel this Game Day party?")) {
      this.eventService.deleteEvent(this.event.eventId).subscribe({
        next: () => this.router.navigate(['/'])
      });
    }
  }

  suggestFood() {
    if (!this.event || !this.newFoodName.trim() || this.event.isFinalized) return;
    this.eventService.addFoodSuggestion(this.event.eventId, { foodName: this.newFoodName }).subscribe({
      next: () => {
        this.newFoodName = '';
        this.loadEvent(this.event.eventId);
      }
    });
  }

   onClaim(foodId: number) {
    this.eventService.claimFood(foodId).subscribe({
      next: () => this.loadEvent(this.event.eventId) // this will refresh UI
    });
  }
}
