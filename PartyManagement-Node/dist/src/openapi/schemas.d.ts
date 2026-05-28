import { OpenAPIRegistry } from '@asteasolutions/zod-to-openapi';
import { z } from 'zod';
export declare const registry: OpenAPIRegistry;
export declare const PartyModelSchema: z.ZodObject<{
    id: z.ZodString;
    officeId: z.ZodString;
    firstName: z.ZodString;
    lastName: z.ZodString;
    address: z.ZodOptional<z.ZodString>;
    description: z.ZodOptional<z.ZodString>;
    phone: z.ZodOptional<z.ZodString>;
    identificationNumber: z.ZodOptional<z.ZodString>;
}, "strip", z.ZodTypeAny, {
    id: string;
    officeId: string;
    firstName: string;
    lastName: string;
    address?: string | undefined;
    description?: string | undefined;
    phone?: string | undefined;
    identificationNumber?: string | undefined;
}, {
    id: string;
    officeId: string;
    firstName: string;
    lastName: string;
    address?: string | undefined;
    description?: string | undefined;
    phone?: string | undefined;
    identificationNumber?: string | undefined;
}>;
export declare const PartyCreateModelSchema: z.ZodObject<{
    firstName: z.ZodString;
    lastName: z.ZodString;
    address: z.ZodOptional<z.ZodString>;
    description: z.ZodOptional<z.ZodString>;
    phone: z.ZodOptional<z.ZodString>;
    identificationNumber: z.ZodOptional<z.ZodString>;
}, "strip", z.ZodTypeAny, {
    firstName: string;
    lastName: string;
    address?: string | undefined;
    description?: string | undefined;
    phone?: string | undefined;
    identificationNumber?: string | undefined;
}, {
    firstName: string;
    lastName: string;
    address?: string | undefined;
    description?: string | undefined;
    phone?: string | undefined;
    identificationNumber?: string | undefined;
}>;
export declare const PartyCountModelSchema: z.ZodObject<{
    clientsCount: z.ZodNumber;
    opposingPartiesCount: z.ZodNumber;
}, "strip", z.ZodTypeAny, {
    clientsCount: number;
    opposingPartiesCount: number;
}, {
    clientsCount: number;
    opposingPartiesCount: number;
}>;
/** Shared header parameter registered for reuse across all operations. */
export declare const OfficeIdHeaderParam: z.ZodString;
//# sourceMappingURL=schemas.d.ts.map