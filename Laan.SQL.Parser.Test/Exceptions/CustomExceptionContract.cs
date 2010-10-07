using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Laan.SQLParser.Test.Exceptions
{
    public class CustomExceptionContract<TException> : ExceptionContract<TException> where TException : System.Exception
    {
        protected override IEnumerable<MbUnit.Framework.Test> GetContractVerificationTests( )
        {
            if ( ImplementsSerialization )
            {
                // Has Serializable attribute?
                yield return CreateSerializableAttributeTest( "HasSerializableAttribute" );

                // Has non-public serialization constructor?
                yield return CreateSerializationConstructorTest( "HasSerializationConstructor" );
            }

            if ( ImplementsStandardConstructors )
            {
                // Is public default constructor well defined?
                yield return CreateNonStandardConstructorTest( "DefaultConstructor",
                                                           "",
                                                           EmptyArray<Type>.Instance,
                                                           new object[][]
                                                           {
                                                               EmptyArray<object>.Instance
                                                           } );

                // Is public single parameter constructor (message) well defined?
                yield return CreateNonStandardConstructorTest( "MessageConstructor",
                                                           "string",
                                                           new Type[] { typeof( string ) },
                                                           new object[][]
                                                           {
                                                               new object[] { null },
                                                               new object[] { "" },
                                                               new object[] { "A message" }
                                                           } );

                // Is public two parameters constructor (message and inner exception) well defined?
                yield return CreateNonStandardConstructorTest( "MessageAndInnerExceptionConstructor",
                                                           "string, Exception",
                                                           new Type[] { typeof( string ), typeof( Exception ) },
                                                           new object[][]
                                                           {
                                                               new object[] { null, null },
                                                               new object[] { "", null },
                                                               new object[] { "A message", null },
                                                               new object[] { null, new Exception( ) },
                                                               new object[] { "", new Exception( ) },
                                                               new object[] { "A message", new Exception( ) }
                                                           } );
            }
        }

        private MbUnit.Framework.Test CreateSerializableAttributeTest( string name )
        {
            return new TestCase( name, () =>
                                {
                                    AssertionHelper.Verify( () =>
                                                           {
                                                               if ( typeof( TException ).IsDefined( typeof( SerializableAttribute ), false ) )
                                                                   return null;

                                                               return new AssertionFailureBuilder( "Expected the exception type to have the [Serializable] attribute." )
                                                               .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                                               .SetStackTrace( Context.GetStackTraceData( ) )
                                                               .ToAssertionFailure( );
                                                           } );
                                } );
        }

        private MbUnit.Framework.Test CreateSerializationConstructorTest( string name )
        {
            return new TestCase( name, () =>
                                {
                                    var constructor = typeof( TException ).GetConstructor(
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                                        new[] { typeof( SerializationInfo ), typeof( StreamingContext ) }, null );

                                    AssertionHelper.Explain( () =>
                                                            Assert.IsNotNull( constructor ),
                                                            innerFailures => new AssertionFailureBuilder(
                                                                "Expected the exception type to have a serialization constructor with signature .ctor(SerializationInfo, StreamingContext)." )
                                                            .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                                            .SetStackTrace( Context.GetStackTraceData( ) )
                                                            .AddInnerFailures( innerFailures )
                                                            .ToAssertionFailure( ) );
                                } );
        }

        private MbUnit.Framework.Test CreateNonStandardConstructorTest( string name, string constructorSignature, 
                                                                    Type[] constructorParameterTypes, object[][] constructorArgumentLists )
        {
            return new TestCase( name, () =>
                                {
                                    var constructor = typeof( TException ).GetConstructor( BindingFlags.Instance | BindingFlags.Public,
                                                                                          null, constructorParameterTypes, null );

                                    AssertionHelper.Explain( () =>
                                                            Assert.IsNotNull( constructor ),
                                                            innerFailures => new AssertionFailureBuilder( String.Format(
                                                                "Expected the exception type to have a standard constructor with signature .ctor({0}).", constructorSignature ) )
                                                            .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                                            .SetStackTrace( Context.GetStackTraceData( ) )
                                                            .AddInnerFailures( innerFailures )
                                                            .ToAssertionFailure( ) );

                                    foreach ( var constructorArgumentList in constructorArgumentLists )
                                    {
                                        TException instance = (TException) constructor.Invoke( constructorArgumentList );
                                        string message = constructorArgumentList.Length > 0 ? (string) constructorArgumentList[ 0 ] : null;
                                        Exception innerException = constructorArgumentList.Length > 1 ? (Exception) constructorArgumentList[ 1 ] : null;

                                        Assert.Multiple( () =>
                                                        {
                                                            AssertionHelper.Explain( () =>
                                                                                    Assert.AreSame( innerException, instance.InnerException ),
                                                                                    innerFailures => new AssertionFailureBuilder(
                                                                                        "The inner exception should be referentially identical to the exception provided in the constructor." )
                                                                                    .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                                                                    .AddRawLabeledValue( "Actual Inner Exception", instance.InnerException )
                                                                                    .AddRawLabeledValue( "Expected Inner Exception", innerException )
                                                                                    .SetStackTrace( Context.GetStackTraceData( ) )
                                                                                    .AddInnerFailures( innerFailures )
                                                                                    .ToAssertionFailure( ) );

                                                            if ( message == null )
                                                            {
                                                                AssertionHelper.Explain( () =>
                                                                                        Assert.IsNotNull( instance.Message ),
                                                                                        innerFailures => new AssertionFailureBuilder(
                                                                                            "The exception message should not be null." )
                                                                                        .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                                                                        .SetStackTrace( Context.GetStackTraceData( ) )
                                                                                        .AddInnerFailures( innerFailures )
                                                                                        .ToAssertionFailure( ) );
                                                            }
                                                            else
                                                            {
                                                                //AssertionHelper.Explain( () =>
                                                                //                        Assert.AreEqual( message, instance.Message ),
                                                                //                        innerFailures => new AssertionFailureBuilder(
                                                                //                            "Expected the exception message to be equal to a specific text." )
                                                                //                        .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                                                //                        .AddLabeledValue( "Actual Message", instance.Message )
                                                                //                        .AddLabeledValue( "Expected Message", message )
                                                                //                        .SetStackTrace( Context.GetStackTraceData( ) )
                                                                //                        .AddInnerFailures( innerFailures )
                                                                //                        .ToAssertionFailure( ) );
                                                            }

                                                            if ( ImplementsSerialization )
                                                            {
                                                                AssertMessageAndInnerExceptionPreservedByRoundTripSerialization( instance );
                                                            }
                                                        } );
                                    }
                                } );
        }

        /// <summary>
        /// Verifies that the <see cref="Exception.Message" /> and 
        /// <see cref="Exception.InnerException" /> properties are preserved by round-trip serialization.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        private void AssertMessageAndInnerExceptionPreservedByRoundTripSerialization( Exception instance )
        {
            Exception result = RoundTripSerialize( instance );

            AssertionHelper.Explain( () =>
                                    Assert.AreEqual( result.Message, instance.Message ),
                                    innerFailures => new AssertionFailureBuilder(
                                        "Expected the exception message to be preserved by round-trip serialization." )
                                    .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                    .AddLabeledValue( "Expected Message", instance.Message )
                                    .AddLabeledValue( "Actual Message ", result.Message )
                                    .SetStackTrace( Context.GetStackTraceData( ) )
                                    .AddInnerFailures( innerFailures )
                                    .ToAssertionFailure( ) );

            AssertionHelper.Verify( () =>
                                   {
                                       if ( result.InnerException == null && instance.InnerException == null )
                                           return null;
                                       if ( result.InnerException != null && instance.InnerException != null
                                            && result.InnerException.GetType( ) == instance.InnerException.GetType( )
                                            && result.InnerException.Message == instance.InnerException.Message )
                                           return null;

                                       return new AssertionFailureBuilder( "The inner exception should be preserved by round-trip serialization." )
                                       .AddRawLabeledValue( "Exception Type", typeof( TException ) )
                                       .AddRawLabeledValue( "Actual Inner Exception", instance.InnerException )
                                       .AddRawLabeledValue( "Expected Inner Exception", result.InnerException )
                                       .SetStackTrace( Context.GetStackTraceData( ) )
                                       .ToAssertionFailure( );
                                   } );
        }
    }
}
