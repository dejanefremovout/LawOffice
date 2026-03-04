/**
 * Hearing Model
 */
export interface Hearing {
  id: string;
  caseId: string;
  courtroom: string | null;
  description: string | null;
  date: string; // ISO string
  held: boolean;  
}
