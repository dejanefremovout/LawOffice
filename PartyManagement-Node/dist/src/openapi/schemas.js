"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.OfficeIdHeaderParam = exports.PartyCountModelSchema = exports.PartyCreateModelSchema = exports.PartyModelSchema = exports.registry = void 0;
const zod_to_openapi_1 = require("@asteasolutions/zod-to-openapi");
const zod_1 = require("zod");
(0, zod_to_openapi_1.extendZodWithOpenApi)(zod_1.z);
exports.registry = new zod_to_openapi_1.OpenAPIRegistry();
exports.PartyModelSchema = exports.registry.register('PartyModel', zod_1.z.object({
    id: zod_1.z.string().openapi({ description: 'Unique identifier of the party.', example: 'a1b2c3d4-...' }),
    officeId: zod_1.z.string().openapi({ description: 'Office the party belongs to.', example: 'office-123' }),
    firstName: zod_1.z.string().openapi({ example: 'John' }),
    lastName: zod_1.z.string().openapi({ example: 'Doe' }),
    address: zod_1.z.string().optional().openapi({ example: '123 Main St' }),
    description: zod_1.z.string().optional().openapi({ example: 'Key client since 2024.' }),
    phone: zod_1.z.string().optional().openapi({ example: '+1-555-0100' }),
    identificationNumber: zod_1.z.string().optional().openapi({ example: 'ID-0001' }),
}).openapi('PartyModel'));
exports.PartyCreateModelSchema = exports.registry.register('PartyCreateModel', zod_1.z.object({
    firstName: zod_1.z.string().openapi({ example: 'John' }),
    lastName: zod_1.z.string().openapi({ example: 'Doe' }),
    address: zod_1.z.string().optional().openapi({ example: '123 Main St' }),
    description: zod_1.z.string().optional().openapi({ example: 'Key client since 2024.' }),
    phone: zod_1.z.string().optional().openapi({ example: '+1-555-0100' }),
    identificationNumber: zod_1.z.string().optional().openapi({ example: 'ID-0001' }),
}).openapi('PartyCreateModel'));
exports.PartyCountModelSchema = exports.registry.register('PartyCountModel', zod_1.z.object({
    clientsCount: zod_1.z.number().int().openapi({ example: 12 }),
    opposingPartiesCount: zod_1.z.number().int().openapi({ example: 7 }),
}).openapi('PartyCountModel'));
/** Shared header parameter registered for reuse across all operations. */
exports.OfficeIdHeaderParam = exports.registry.registerParameter('XOfficeId', zod_1.z.string().openapi({
    param: {
        name: 'x-office-id',
        in: 'header',
        required: true,
        description: 'Tenant identifier. Scopes all queries to the specified office.',
    },
    example: 'office-123',
}));
//# sourceMappingURL=schemas.js.map