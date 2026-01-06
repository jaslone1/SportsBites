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

  isEditingEvent = false;
  editingFoodId: number | null = null;
  tempEditValue = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.currentUserName = this.authService.getCurrentUser();
    this.currentUserId  = this.authService.getUserId();

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

  onUnclaim(foodId: number) {
    this.eventService.unclaimFood(foodId).subscribe({
      next: () => this.loadEvent(this.event.eventId),
      error: (err) => console.error("Error unclaiming food:", err)
    });
  }
// Edit Event
  onSaveEvent() {
    this.eventService.updateEvent(this.event.eventId, this.event).subscribe({
      next: () => this.isEditingEvent = false
    });
  }

// Edit Food
  startEditFood(food: any) {
    this.editingFoodId = food.foodSuggestionId;
    this.tempEditValue = food.foodName;
  }

  saveFoodEdit(foodId: number) {
    if (!this.tempEditValue.trim()) return;
    this.eventService.updateFoodName(foodId, this.tempEditValue).subscribe({
      next: () => {
        this.editingFoodId = null;
        this.loadEvent(this.event.eventId);
      },
      error: (err) => console.error("Error updating food:", err)
    });
  }

  // Check if current user is the host
  isHost(): boolean {
    if (!this.event || !this.currentUserId) return false;
    return String(this.event.hostUserId) === String(this.currentUserId);
  }

// Check if current user suggested the specific food
  canEditFood(food: any): boolean {
    if (!this.currentUserId || !food) return false;
    const isSuggester = String(food.suggestedByUserId) === String(this.currentUserId);
    return isSuggester || this.isHost();
  }

  // Check if current user is the one who claimed the item
  isClaimant(food: any): boolean {
    if (!this.currentUserId || !food.claimedByUserId) return false;
    return String(food.claimedByUserId) === String(this.currentUserId);
  }
}
