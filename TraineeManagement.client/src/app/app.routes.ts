import { Routes } from '@angular/router';

import { Login } from './features/login/login';
import { Dashboard } from './features/dashboard/dashboard';
import { MainLayout } from './core/layout/main-layout/main-layout';
import { TraineeList } from './features/trainees/trainee-list/trainee-list';
import { MentorList } from './features/mentors/mentor-list/mentor-list';
import { LearningTaskList } from './features/learning-tasks/learning-task-list/learning-task-list';
import { AssignmentList } from './features/assignments/assignment-list/assignment-list';
import { SubmissionList } from './features/submissions/submission-list/submission-list';
import { ReviewList } from './features/reviews/review-list/review-list';

import { authGuard } from './core/guards/auth-guard';

import { TraineeForm } from './features/trainees/trainee-form/trainee-form';

export const routes: Routes = [
  {
    path: 'login',
    component: Login,
  },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard',
      },
      {
        path: 'dashboard',
        component: Dashboard,
      },
      {
        path: 'trainees',
        component: TraineeList,
      },
      {
        path: 'trainees/add',
        component: TraineeForm,
      },
      {
        path: 'trainees/:id/edit',
        component: TraineeForm
      },
      {
        path: 'mentors',
        component: MentorList,
      },
      {
        path: 'learning-tasks',
        component: LearningTaskList,
      },
      {
        path: 'assignments',
        component: AssignmentList,
      },
      {
        path: 'submissions',
        component: SubmissionList,
      },
      {
        path: 'reviews',
        component: ReviewList,
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];