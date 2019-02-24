using System;
using System.Linq;
using DynamicData.Tests.Domain;
using FluentAssertions;
using Xunit;

namespace DynamicData.Tests.List
{
    
    public class FilterOnPropertyFixture
    {
        [Fact]
        public void InitialValues()
        {
            var people = Enumerable.Range(1, 100).Select(i => new Person("Name" + i, i)).ToArray();
            using (var stub = new FilterPropertyStub())
            {
                stub.Source.AddRange(people);

                stub.Results.MessageCount().Should().Be(2);
                stub.Results.DataCount().Should().Be(82);
                stub.Results.Data.Items.ShouldAllBeEquivalentTo(people.Skip(18));
            }
        }

        [Fact]
        public void ChangeAValueToMatchFilter()
        {
            var people = Enumerable.Range(1, 100).Select(i => new Person("Name" + i, i)).ToArray();
            using (var stub = new FilterPropertyStub())
            {
                stub.Source.AddRange(people);

                people[20].Age = 10;

                stub.Results.MessageCount().Should().Be(3);
                stub.Results.DataCount().Should().Be(81);
            }
        }

        [Fact]
        public void ChangeAValueToNoLongerMatchFilter()
        {
            var people = Enumerable.Range(1, 100).Select(i => new Person("Name" + i, i)).ToArray();
            using (var stub = new FilterPropertyStub())
            {
                stub.Source.AddRange(people);

                people[10].Age = 20;

                stub.Results.MessageCount().Should().Be(3);
                stub.Results.DataCount().Should().Be(83);
            }
        }

        [Fact]
        public void ChangeAValueSoItIsStillInTheFilter()
        {
            var people = Enumerable.Range(1, 100).Select(i => new Person("Name" + i, i)).ToArray();
            using (var stub = new FilterPropertyStub())
            {
                stub.Source.AddRange(people);

                people[50].Age = 100;
                stub.Results.MessageCount().Should().Be(3);
                stub.Results.DataCount().Should().Be(82);
            }
        }

        [Fact]
        public void Clear()
        {
            var people = Enumerable.Range(1, 100).Select(i => new Person("Name" + i, i)).ToArray();
            using (var stub = new FilterPropertyStub())
            {
                stub.Source.AddRange(people);
                stub.Source.Clear();

                stub.Results.Data.Count.Should().Be(0);
            }
        }


        [Fact]
        public void RemoveRange()
        {
            var people = Enumerable.Range(1, 100).Select(i => new Person("Name" + i, i)).ToArray();
            using (var stub = new FilterPropertyStub())
            {
                stub.Source.AddRange(people);
                stub.Source.RemoveRange(89,10);

                stub.Results.Data.Count.Should().Be(72);
            }
        }

        private class FilterPropertyStub : IDisposable
        {
            public ISourceList<Person> Source { get; } = new SourceList<Person>();
            public ChangeSetAggregator<Person> Results { get; }


            public FilterPropertyStub()
            {
                Results = new ChangeSetAggregator<Person>(Source.Connect().FilterOnProperty(p => p.Age, p => p.Age > 18));
            }

            public void Dispose()
            {
                Source.Dispose();
                Results.Dispose();
            }
        }
    }
}