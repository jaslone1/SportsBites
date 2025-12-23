import { Component, OnInit } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';

@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html', // Fixed: was pointing to details
  standalone: false
})
export class EventListComponent implements OnInit { // Fixed: was EventDetailsComponent
  events: EventDto[] = [];

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents() {
    this.eventService.getAllEvents().subscribe({
      next: (data) => this.events = data,
      error: (err) => console.error(err)
    });
  }

  get upcomingEvents() {
    const today = new Date();
    return this.events.filter(e => new Date(e.eventDate) >= today);
  }

  get pastEvents() {
    const today = new Date();
    return this.events.filter(e => new Date(e.eventDate) < today);
  }
}
