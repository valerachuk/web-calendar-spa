import { UserInfo } from '../interfaces/user-info.interface';
import { NotificationTime } from '../enums/notification-time.enum';
import { Reiteration } from '../enums/reiteration.enum';

export interface CalendarEvent {
  id: number;
  name: string;
  venue: string | null;
  calendarId: number;
  startDateTime: string;
  endDateTime: string;
  notificationTime: NotificationTime | null;
  reiteration: Reiteration | null;
  guests: UserInfo[] | null;
  fileId?:number;
}