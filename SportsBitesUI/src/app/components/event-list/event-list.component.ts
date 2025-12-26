import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';

@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html', // Fixed: was pointing to details
  standalone: false
})
export class EventListComponent implements OnInit { // Fixed: was EventDetailsComponent
  upcomingEvents: any[] = [];
  pastEvents: any[] = [];

  constructor(
    private eventService: EventService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.getEvents().subscribe({
      next: (data: any[]) => {
        const now = new Date();

        this.upcomingEvents = data.filter(e => new Date(e.eventDate) >= now);
        this.pastEvents = data.filter(e => new Date(e.eventDate) < now);

        console.log('Events Split:', { upcoming: this.upcomingEvents.length, past: this.pastEvents.length });

        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('List load failed', err);
      }
    });
  }
}
