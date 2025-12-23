import { Component, OnInit } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-event-details',
  templateUrl: './event-details.component.html',
  standalone: false
})
export class EventDetailsComponent implements OnInit {
  event: any = null;
  voterName: string = 'Jared';

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute // Changed from croute to route for simplicity
  ) {}

  ngOnInit(): void {
    // 1. Get the ID from the URL
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadEvent(id);
    }
  }

  loadEvent(id: number) {
    this.eventService.getEvent(id, this.voterName).subscribe({
      next: (data) => {
        this.event = { ...data };
      },
      error: (err) => {
        console.error("API Error:", err);
      }
    });
  }

  onVote(foodId: number) {
    this.eventService.upvoteFood(foodId, this.voterName).subscribe(() => {
      // 2. We need the ID again to refresh the list
      const id = Number(this.route.snapshot.paramMap.get('id'));
      this.loadEvent(id);
    });
  }
}
