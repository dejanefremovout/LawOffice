export interface CaseUpdateForm {
  identificationNumber: string;
  description: string | null;
  year: number | null;
  city: string | null;
  competentCourt: string | null;
  judge: string | null;
  active: boolean;
  clientIds: string[];
  opposingPartyIds: string[];
}
