import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';
import { ActivatedRoute } from '@angular/router';
import { filter, map, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-details-view',
  templateUrl: './event-details.component.html',
  standalone: false
})
export class EventDetailsComponent implements OnInit {
  event: any = null;
  voterName: string = 'Jared';
  newFoodName = '';

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      console.log("Route Params Received:", id);

      if (id && id !== '0') {
        this.loadEvent(Number(id));
      } else {
        console.warn("Ghost detected: No ID in URL. Blocking API call.");
      }
    });
  }

  loadEvent(id: number) {
    this.eventService.getEvent(id, this.voterName).subscribe({
      next: (data) => {
        console.log("Data received", data);
        this.event = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error("API Error:", err);
      }
    });
  }

  onVote(foodId: number) {
    this.eventService.upvoteFood(foodId, this.voterName).subscribe(() => {
      if (this.event) {
        this.loadEvent(this.event.eventId);
      }
    });
  }


  suggestFood() {
    if (!this.event || !this.newFoodName.trim()) return;
    const suggestion = {
      foodName: this.newFoodName,
      suggestedByName: this.voterName // Make sure this matches 'suggestedByName' in your DTO
    };

    this.eventService.addFoodSuggestion(this.event.eventId, suggestion).subscribe({
      next: () => {
        this.newFoodName = '';
        this.loadEvent(this.event.eventId);
      }
    });
  }


}
