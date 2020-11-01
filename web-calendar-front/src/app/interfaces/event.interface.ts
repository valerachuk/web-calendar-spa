export interface CalendarEvent {
  id: number;
  name: string;
  venue: string | null;
  calendarId: number;
  startDateTime: string;
  endDateTime: string;
  notificationTime: NotificationTime | null;
  reiteration: Reiteration | null;
}

export enum NotificationTime {
  In10Minutes = 10,
  In15Minutes = 15,
  In30Minutes = 30,
  In1Hour = 60
}

export enum Reiteration {
  Daily = 1,
  Weekly = 7
}