/**
 * Lawyer Model
 */
export interface Lawyer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  invitationCode?: string | null;
}
