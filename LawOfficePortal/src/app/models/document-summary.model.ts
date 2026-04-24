export type SummaryDepth = 'short' | 'detailed';

export interface DocumentSummary {
  documentFileId: string;
  summaryDepth: SummaryDepth;
  shortSummary: string;
  keyPoints: string[];
  actionItems: string[];
}
