import { computed } from '@angular/core';

import {
  patchState,
  signalStore,
  withComputed,
  withMethods,
  withState,
} from '@ngrx/signals';
import { nanoid } from 'nanoid';

export type NotificationType =
  | 'alert'
  | 'success'
  | 'error'
  | 'info'
  | 'warning';

export interface AppNotification {
  id: string;
  message: string;
  type: NotificationType;
  timeout?: number;
}

export const NotificationStore = signalStore(
  { providedIn: 'root' },
  withState({
    notifications: [] as AppNotification[],
  }),

  withComputed((store) => ({
    count: computed(() => store.notifications().length),
    hasNotifications: computed(() => store.notifications().length > 0),
  })),

  withMethods((store) => ({
    add(notification: Omit<AppNotification, 'id'>) {
      const id = nanoid();
      const newNotification = { ...notification, id };

      patchState(store, (state) => ({
        notifications: [...state.notifications, newNotification],
      }));

      if (notification.timeout) {
        setTimeout(() => {
          this.remove(id);
        }, notification.timeout);
      }
    },

    remove(id: string) {
      patchState(store, (state) => ({
        notifications: state.notifications.filter((n) => n.id !== id),
      }));
    },

    clearAll() {
      patchState(store, { notifications: [] });
    },
  })),
);
