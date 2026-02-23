/**
 * Document Model
 */
export interface Document {
  id: string;
  caseId: string;
  officeId: string;
  name: string;
  uri: string | null;
}
