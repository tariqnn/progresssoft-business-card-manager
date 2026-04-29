import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-notice',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './notice.component.html',
  styleUrl: './notice.component.css'
})
export class NoticeComponent {
  private readonly notifications = inject(NotificationService);
  readonly notice = this.notifications.notice;

  dismiss(): void {
    this.notifications.dismiss();
  }
}
