import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';

import { NotificationsStore } from '../../stores/notifications.store';

@Component({
  selector: 'app-notifications',
  imports: [CommonModule],
  templateUrl: 'notifications.component.html',
})
export class NotificationsComponent {
  readonly store = inject(NotificationsStore);

  getAlertClass(type: string) {
    return {
      'alert-success': type === 'success',
      'alert-error': type === 'error',
      'alert-info': type === 'info',
      'alert-warning': type === 'warning',
    };
  }

  getButtonClass(type: string) {
    return {
      'btn-success': type === 'success',
      'btn-error': type === 'error',
      'btn-info': type === 'info',
      'btn-warning': type === 'warning',
    };
  }
}
