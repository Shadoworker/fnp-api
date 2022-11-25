/**
 * fnp controller
 */

import { factories } from '@strapi/strapi'

export default factories.createCoreController('api::fnp.fnp', ({ strapi }) =>  ({
    // Method 1: Creating an entirely custom action
    async exampleAction(ctx) {
      try {
        ctx.body = 'ok';
      } catch (err) {
        ctx.body = err;
      }
    },
  
    async index(ctx) { // called by GET /hello 
        ctx.body = 'Hello World!'; // we could also send a JSON
      },
    // Method 2: Wrapping a core action (leaves core logic in place)
    async find(ctx) {
      // some custom logic here
      ctx.query = { ...ctx.query, local: 'en' }
      
      // Calling the default core action
      const { data, meta } = await super.find(ctx);
  
      // some more custom logic
      meta.date = Date.now()
  
      return { data, meta };
    },
  
    // Method 3: Replacing a core action
    async findOne(ctx) {
      const { id } = ctx.params;
      const { query } = ctx;
  
      const entity = await strapi.service('api::fnp.fnp').findOne(id, query);
      const sanitizedEntity = await this.sanitizeOutput(entity, ctx);
  
      return this.transformResponse(sanitizedEntity);
    }
  }));
   